﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamblingProject.Models;
using MongoDB.Driver;

namespace GamblingProject.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient client)
        {
            var database = client.GetDatabase("GamblingDB");

            _users = database.GetCollection<User>("Users");
        }

        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }

        public User Get(string id)
        {
            return _users.Find(user => user.Id.ToString() == id).FirstOrDefault();
        }

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(Guid id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(user => user.Id.ToString() == id);
        }

        public void DepositEth(string id, double eth)
        {
            var user = _users.Find(user => user.Id.ToString() == id).FirstOrDefault();
            user.EthAmount += eth;
            _users.ReplaceOne(user => user.Id.ToString() == id, user);
        }

        public void WithdrawEth(string id, double eth)
        {
            var user = _users.Find(user => user.Id.ToString() == id).FirstOrDefault();
            user.EthAmount -= eth;
            _users.ReplaceOne(user => user.Id.ToString() == id, user);
        }

        public async Task<ResponseDto> ConvertEthToTokens(string id, double eth)
        {
            var user = _users.Find(user => user.Id.ToString() == id).FirstOrDefault();

            if (user.EthAmount < eth)
                return new ResponseDto("Not enough ETH", "error", "failed");
            user.EthAmount -= eth;
            user.AssetTokens += eth * await GetCryptoValue.GetEthPriceAsync() / 10;
            _users.ReplaceOne(user => user.Id.ToString() == id, user);
            return new ResponseDto("ETH converted to tokens", "success");
        }

        public async void ConvertTokensToEth(string id, double tokens)
        {
            var user = _users.Find(user => user.Id.ToString()== id).FirstOrDefault();
            user.AssetTokens -= tokens;
            user.EthAmount += tokens * 10 / await GetCryptoValue.GetEthPriceAsync();
            _users.ReplaceOne(user => user.Id.ToString() == id, user);
        }
    }
}
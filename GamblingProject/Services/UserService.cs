using System.Collections.Generic;
using System.Threading.Tasks;
using GamblingProject.Models;
using MongoDB.Driver;

namespace GamblingProject.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IGamblingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>("Users");
        }

        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }

        public User Get(string id)
        {
            return _users.Find(user => user.Id == id).FirstOrDefault();
        }

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(user => user.Id == id);
        }

        public void DepositEth(string id, double eth)
        {
            var user = _users.Find(user => user.Id == id).FirstOrDefault();
            user.EthAmount += eth;
            _users.ReplaceOne(user => user.Id == id, user);
        }

        public void WithdrawEth(string id, double eth)
        {
            var user = _users.Find(user => user.Id == id).FirstOrDefault();
            user.EthAmount -= eth;
            _users.ReplaceOne(user => user.Id == id, user);
        }

        public async Task<ResponseDto> ConvertEthToTokens(string id, double eth)
        {
            var user = _users.Find(user => user.Id == id).FirstOrDefault();

            if (user.EthAmount < eth)
                return new ResponseDto("Not enough ETH","error","failed");
            user.EthAmount -= eth;
            user.Tokens += eth * await GetCryptoValue.GetEthPriceAsync() / 10;
            _users.ReplaceOne(user => user.Id == id, user);
            return new ResponseDto("ETH converted to tokens","success");
        }

        public async void ConvertTokensToEth(string id, double tokens)
        {
            var user = _users.Find(user => user.Id == id).FirstOrDefault();
            user.Tokens -= tokens;
            user.EthAmount += tokens * 10 / await GetCryptoValue.GetEthPriceAsync();
            _users.ReplaceOne(user => user.Id == id, user);
        }
    }
}
﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GamblingProject.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        public double Tokens { get; set; }
        public double EthAmount { get; set; }

        public double TotalAssets()
        {
            return Tokens * 10;
        }
    }
}
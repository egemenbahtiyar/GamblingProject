using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace GamblingProject.Models
{
    [CollectionName("Users")]
    public class User : MongoIdentityUser<Guid>
    {
        public double AssetTokens { get; set; }
        public double EthAmount { get; set; }

        public double TotalAssets()
        {
            return AssetTokens * 10;
        }
    }
}
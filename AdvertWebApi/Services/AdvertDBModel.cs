using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertWebApi.Services
{
    [DynamoDBTable("Advert")]
    public class AdvertDBModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public double Price { get; set; }
        [DynamoDBProperty]
        public DateTime CreatedDateTime { get; set; }
        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }


        
    }
}

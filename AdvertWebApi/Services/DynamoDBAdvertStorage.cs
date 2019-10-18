using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using Amazon;
using AutoMapper;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertWebApi.Services

{
    public class DynamoDBAdvertStorage :IAdvertStorageService
    {
        private readonly IMapper _mapper;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;


        public DynamoDBAdvertStorage(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<string> Add(AdvertModel model)
        {
            var dbmodel = _mapper.Map<AdvertDBModel>(model);
            dbmodel.Id = Guid.NewGuid().ToString();
            dbmodel.CreatedDateTime = DateTime.UtcNow;
            dbmodel.Status = AdvertStatus.Pending;

            using (var cleint = new AmazonDynamoDBClient(bucketRegion))
            {
                using (var context = new DynamoDBContext(cleint))
                {

                    await context.SaveAsync(dbmodel);
                }
            }

            return dbmodel.Id;

        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            
            model.Status = AdvertStatus.Active;
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDBModel>(model.Id);
                    if (model.Status == AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                    {
                        await context.DeleteAsync(record);
                    }

                   
                }
            }

            return true;

        }

        public async Task<bool> CheckHealthAsync()
        {
            using (var client = new AmazonDynamoDBClient())
            {
                var tableData = await client.DescribeTableAsync("Advert");
                return string.Compare(tableData.Table.TableStatus, "active", true) == 0;
            }
        }
    }
}

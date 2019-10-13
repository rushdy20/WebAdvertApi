using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertWebApi.Services

{
    public class DynamoDBAdvertStorage :IAdvertStorageService
    {
        private readonly IMapper _mapper;
       

        public DynamoDBAdvertStorage(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<string> Add(AdvertModel model)
        {
            var dbmodel = _mapper.Map<AdvertDBModel>(model);
            dbmodel.Id = new Guid().ToString();
            dbmodel.CreatedDateTime = DateTime.UtcNow;
            dbmodel.Status = AdvertStatus.Pending;

            using (var cleint = new AmazonDynamoDBClient())
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
            using (var cleint = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(cleint))
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
    }
}

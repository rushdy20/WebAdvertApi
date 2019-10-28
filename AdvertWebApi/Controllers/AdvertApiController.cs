using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AdvertApi.Models.Messages;
using AdvertWebApi.Services;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AdvertWebApi.Controllers
{
    [Route("api/adverts/v1")]
    [ApiController]
    public class AdvertApiController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;
        private readonly IConfiguration _configuration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;

        public AdvertApiController(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(201,Type =typeof(CreateAdvertResponse))]
        public async  Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.Add(model);

            }
            catch (KeyNotFoundException )
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
                
            }

            return StatusCode(201, new CreateAdvertResponse {Id = recordId});

        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
           
            try
            {
                 await _advertStorageService.Confirm(model);
                 await RaiseAdvertConformerMessage(model);


            }
            
            catch (Exception e)
            {
                return StatusCode(500, e.Message);

            }

            return new OkResult();

        }

        private async Task RaiseAdvertConformerMessage(ConfirmAdvertModel model)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");
            var dbmodel = await _advertStorageService.GetByIdAsync(model.Id);
            using (var client = new AmazonSimpleNotificationServiceClient(bucketRegion))
            {
                var message = new AdvertConfirmMessage
                {
                    Id = model.Id,
                    Title = dbmodel.Title
                };
                var messageJson = JsonConvert.SerializeObject(message);
                await client.PublishAsync(topicArn, messageJson);
            }
        }
    }
}
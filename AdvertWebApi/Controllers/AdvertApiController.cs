using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AdvertWebApi.Services;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvertWebApi.Controllers
{
    [Route("api/adverts/v1")]
    [ApiController]
    public class AdvertApiController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertApiController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
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

            }
            
            catch (Exception e)
            {
                return StatusCode(500, e.Message);

            }

            return new OkResult();

        }


    }
}
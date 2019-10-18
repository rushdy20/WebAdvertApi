using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvertWebApi.Services;
using Microsoft.Extensions.HealthChecks;

namespace AdvertWebApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IAdvertStorageService _storageService;


        public StorageHealthCheck(IAdvertStorageService storageService)
        {
            _storageService = storageService;
        }
        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var isStorageOk = await _storageService.CheckHealthAsync();
            return HealthCheckResult.FromStatus(isStorageOk ? CheckStatus.Healthy : CheckStatus.Unhealthy,"");
        }
    }
}

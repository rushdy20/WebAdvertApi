using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace AdvertWebApi.Services
{
  public  interface IAdvertStorageService
  {
      Task<string> Add(AdvertModel model);
      Task<bool> Confirm(ConfirmAdvertModel model);
      Task<bool> CheckHealthAsync();
      Task<AdvertModel> GetByIdAsync(string id);

  }
}

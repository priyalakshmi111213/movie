using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieWebApp.Services
{
    public interface IBaseService
    {
        Task<HttpResponseMessage> GetDataFromExternalAPI(string url);
        List<string> GetAllProviders();

    }
}

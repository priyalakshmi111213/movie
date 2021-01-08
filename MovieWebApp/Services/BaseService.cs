using Microsoft.Extensions.Configuration;
using MovieWebApp.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MovieWebApp.Services
{
    public class BaseService :IBaseService
    {
        readonly IConfiguration _configuration;
        private static Logger logger = LogManager.GetCurrentClassLogger();
       
        public BaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get Data From External API 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDataFromExternalAPI(string url)
        {
            HttpResponseMessage responseMessage=null;
            try
            {
                var baseUrl = _configuration[ConfigurationConstants.APIUrl];
                var headerKey = _configuration.GetSection(ConfigurationConstants.Header).GetSection(ConfigurationConstants.Key).Value;
                var headerValue = _configuration.GetSection(ConfigurationConstants.Header).GetSection(ConfigurationConstants.Value).Value;
                var handler = new TimeoutHandler
                {
                    DefaultTimeout = TimeSpan.FromSeconds(100),
                    InnerHandler = new HttpClientHandler()
                };
                using (var cts = new CancellationTokenSource())
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Add(headerKey, headerValue);
                    logger.Info("Requesting for Data {0}", url);
                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + url);
                    responseMessage = await client.SendAsync(request, cts.Token);
                    responseMessage.EnsureSuccessStatusCode();
                    logger.Info("Response from External source =>" + responseMessage.ToString());
                }
            }
            catch (Exception e)
            {
                logger.Error(e,"Unable to Process Request");
            }
            return responseMessage;
        }

        /// <summary>
        /// Get All Providers from Configuration
        /// </summary>
        /// <returns>List of Providers Configured</returns>
        public List<string> GetAllProviders()
        {
            try
            {
                var providers = _configuration.GetSection("Providers:MovieProviders").Get<List<string>>();
                if (providers != null)
                {
                    return providers;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Unable to Fetch Providers");
            }
            return new List<string>();
        }

    }
}

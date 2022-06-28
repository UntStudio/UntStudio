using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UntStudio.Bootstrapper.API;

namespace UntStudio.Bootstrapper
{
    internal sealed class Bootsrapper : IBootsrapper
    {
        private const string UnloadLoaderRequest = "https://localhost:7192/bootstrapper/unloadLoader";



        public async Task<ServerResult> GetUnloadLoaderAsync(string key, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            httpClient.DefaultRequestHeaders.Add("Key", key);

            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await httpClient.GetAsync(string.Format(UnloadLoaderRequest, key), cancellationToken);

                string responseText = await responseMessage.Content.ReadAsStringAsync();

                RequestResponse response = null;
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(response);
                }

                return new ServerResult(Convert.FromBase64String(responseText));
            }
            catch (HttpRequestException ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
                if (responseMessage != null)
                {
                    return new ServerResult(responseMessage.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            return new ServerResult();
        }
    }
}

using Newtonsoft.Json;
using UntStudio.Bootstrapper.API;

namespace UntStudio.Bootstrapper
{
    internal sealed class Bootsrapper : IBootsrapper
    {
        private const string UnloadLoaderRequest = "https://localhost:7192/bootstrapper/unloadLoader?key={0}";



        public async Task<ServerResult> GetUnloadLoaderAsync(string key, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "UntStudio.Loader");

            try
            {
                string responseText = await httpClient.GetStringAsync(string.Format(UnloadLoaderRequest, key), cancellationToken);

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
                return new ServerResult(ex.StatusCode);
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            return new ServerResult();
        }
    }
}

using Newtonsoft.Json;
using System.Reflection;
using UntStudio.Loader.API;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Servers
{
    internal sealed class Server : IServer
    {
        private readonly ILogging logging;

        private const string UnloadPluginRequest = "https://localhost:7192/api/Bootstrapper/unload?loaderBytes={0}&key={1}&name={2}";



        public Server(ILogging logging)
        {
            this.logging = logging;
        }



        public async Task<ServerResult> GetUnloadPluginAsync(string key, string name, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "UntStudio.Loader");

            try
            {
                string responseText = await httpClient.GetStringAsync(string.Format(UnloadPluginRequest, 
                    File.ReadAllBytes(Assembly.GetExecutingAssembly().Location), 
                    key,
                    name), cancellationToken);

                RequestResponse response = null;
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(response);
                }

                return new ServerResult(Convert.FromBase64String(responseText));
            }
            catch (HttpRequestException ex)
            {
                this.logging.Log($"An error occured while getting loader. {ex}");
                return new ServerResult(ex.StatusCode);
            }
            catch (Exception ex)
            {
                this.logging.Log($"An error occured while getting loader. {ex}");
            }
            return new ServerResult();
        }
    }
}

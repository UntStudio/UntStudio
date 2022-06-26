using Newtonsoft.Json;
using System.Reflection;
using UntStudio.Loader.API;

namespace UntStudio.Loader.Servers
{
    internal sealed class Server
    {
        private const string GetPluginRequest = "https://localhost:7192/api/plugins/getplugin?loaderBytes={0}&key={1}";



        public async Task<ServerResult> GetPluginAsync(string key)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "UntStudio.Loader");

            try
            {
                string responseText = await httpClient.GetStringAsync(string.Format(GetPluginRequest,
                    File.ReadAllBytes(Assembly.GetExecutingAssembly().Location),
                    key));

                RequestResponse response = null;
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(response);
                }

                Plugin plugin = null;
                if ((plugin = JsonConvert.DeserializeObject<Plugin>(responseText)) != null)
                {
                    return new ServerResult(plugin);
                }
            }
            catch (HttpRequestException ex)
            {
                return new ServerResult(ex.StatusCode);
            }
            return null;
        }

        public async Task<ServerResult> GetLoaderAsync()
        {
            return null;
        }
    }
}

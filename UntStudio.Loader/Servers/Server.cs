using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.API;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Servers
{
    public sealed class Server : IServer
    {
        private readonly ILogging logging;

        private const string UnloadPluginRequest = "https://localhost:7192/api/Bootstrapper/unload?loaderBytes={0}&key={1}&name={2}";



        public Server(ILogging logging)
        {
            this.logging = logging;
        }



        public async Task<ServerResult> GetUnloadPluginAsync(string key, string name, CancellationToken cancellationToken = default)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            this.logging.Log("SERVER LOADER FLAGS: #0");
            string responseText = null;
            try
            {
                this.logging.Log("SERVER LOADER FLAGS: #1");
                /*responseText = await webClient.DownloadStringTaskAsync(new Uri(string.Format(UnloadPluginRequest,
                    File.ReadAllBytes(Assembly.GetExecutingAssembly().Location),
                    key,
                    name)));*/


                responseText = await webClient.DownloadStringTaskAsync(new Uri(string.Format(UnloadPluginRequest,
                    key,
                    name)));

                this.logging.Log("SERVER LOADER FLAGS: #2");

                RequestResponse response = null;
                this.logging.Log("SERVER LOADER FLAGS: #3");
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    this.logging.Log("SERVER LOADER FLAGS: #4");
                    return new ServerResult(response);
                }

                this.logging.Log("SERVER LOADER FLAGS: #5");
            }
            catch (JsonReaderException)
            {
                if (responseText != null)
                {
                    this.logging.Log("SERVER LOADER FLAGS: #6");
                    return new ServerResult(Convert.FromBase64String(responseText));
                }
                this.logging.Log("SERVER LOADER FLAGS: #7");
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                this.logging.LogError(ex, "An error occured while getting plugin");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                this.logging.LogError(ex, "An error occured while getting plugin");
            }
            catch (Exception ex)
            {
                this.logging.LogError(ex, "An error occured while getting plugin");
            }
            this.logging.Log("SERVER LOADER FLAGS: #8");
            return new ServerResult();
        }
    }
}

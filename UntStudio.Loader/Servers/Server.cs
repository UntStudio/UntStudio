using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.API;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Servers
{
    public sealed class Server : IServer
    {
        private readonly ILogging logging;
        
        private const string UnloadPluginRequest = "https://localhost:5001/pluginsubscriptions/unload?name={0}";



        public Server(ILogging logging)
        {
            this.logging = logging;
        }



        public async Task<ServerResult> GetUnloadPluginAsync(string key, string name, CancellationToken cancellationToken = default)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            string responseText = null;
            try
            {
                responseText = await webClient.DownloadStringTaskAsync(new Uri(string.Format(UnloadPluginRequest, name)));
                RequestResponse response = null;
                if (responseText != null)
                {
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }
            }
            catch (JsonReaderException)
            {
                if (responseText != null)
                {
                    return new ServerResult(Convert.FromBase64String(responseText));
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    this.logging.LogException(ex, "License server is down, sorry about that.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    this.logging.Log("Loader version is outdated.");
                }

                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure)
                {
                    this.logging.Log("Connection failure, server is down.");
                }
                else
                {
                    this.logging.Log("Couldn`t connect to license server. Please, check your internet connection and firewall rules.");
                }

                this.logging.Log("Couldn`t connect to license server. Please, check your internet connection and firewall rules.");
            }
            catch (Exception ex)
            {
                this.logging.LogException(ex, "An error occured while getting plugin!");
            }
            return new ServerResult();
        }
    }
}

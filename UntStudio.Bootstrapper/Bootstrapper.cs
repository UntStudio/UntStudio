using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.Models;

namespace UntStudio.Bootstrapper
{
    internal sealed class Bootstrapper : IBootstrapper
    {
        private const string GetUnloadLoaderRequest = "https://localhost:5001/bootstrapper/unloadLoader";
        
        private const string GetLoaderEntryPointRequest = "https://localhost:5001/bootstrapper/getloaderentrypoint";

        private const string PutBlockPluginRequest = "https://localhost:5001/pluginsubscriptions/block?name={0}";

        private const string PutUnblockPluginRequest = "https://localhost:5001/pluginsubscriptions/unblock?name={0}";



        public async Task<ServerResult> GetUnloadLoaderAsync(string key)
        {
            WebClient webClient = new WebClient();

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            string responseText = null;
            try
            {
                responseText = await webClient.DownloadStringTaskAsync(GetUnloadLoaderRequest);
                RequestResponse response = null;

                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(response);
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
                    Rocket.Core.Logging.Logger.Log("License server is down, sorry about that.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Rocket.Core.Logging.Logger.Log("Loader version is outdated.");
                }
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure)
                {
                    Rocket.Core.Logging.Logger.Log("Connection failure, server is down.");
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log("Couldn`t connect to license server. Please, check your internet connection and firewall rules.");
                }

                Rocket.Core.Logging.Logger.Log("Couldn`t connect to license server. Please, check your internet connection and firewall rules.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, $"An error occured while getting loader.");
            }
            return null;
        }

        public async Task<ServerResult> GetLoaderEntryPointAsync(string key)
        {
            WebClient webClient = new WebClient();

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            string responseText = null;
            try
            {
                responseText = await webClient.DownloadStringTaskAsync(GetLoaderEntryPointRequest);
                LoaderEntryPoint loaderEntryPoint = null;
                if ((loaderEntryPoint = JsonConvert.DeserializeObject<LoaderEntryPoint>(responseText)) != null)
                {
                    return new ServerResult(loaderEntryPoint);
                }
            }
            catch (JsonReaderException)
            {
                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            return null;
        }

        public async Task<ServerResult> PutBlockPluginAsync(string key, string name)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            try
            {
                string responseText = await webClient.DownloadStringTaskAsync(string.Format(PutBlockPluginRequest, name));
                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            return null;
        }

        public async Task<ServerResult> PutUnblockPluginAsync(string key, string name)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            webClient.Headers.Add("Key", key);

            try
            {
                string responseText = await webClient.DownloadStringTaskAsync(string.Format(PutUnblockPluginRequest, name));
                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error occured while getting loader.");
            }
            return null;
        }
    }
}

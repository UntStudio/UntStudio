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
        //private const string UnloadLoaderRequest = "https://localhost:7192/bootstrapper/unloadLoader";
        private const string GetUnloadLoaderRequest = "https://localhost:5001/bootstrapper/unloadLoader";
        
        private const string GetLoaderEntryPointRequest = "https://localhost:5001/bootstrapper/getloaderentrypoint";

        private const string PutBlockPluginRequest = "https://localhost:5001/pluginsubscriptions/block?name={0}";

        private const string PutUnblockPluginRequest = "https://localhost:5001/pluginsubscriptions/unblock?name={0}";



        public async Task<ServerResult> GetUnloadLoaderAsync(string key)
        {
            Rocket.Core.Logging.Logger.Log("FLAG #1");
            WebClient webClient = new WebClient();
            Rocket.Core.Logging.Logger.Log("FLAG #2");

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            Rocket.Core.Logging.Logger.Log("FLAG #3");
            webClient.Headers.Add("Key", key);
            Rocket.Core.Logging.Logger.Log("FLAG #4");

            string responseText = null;
            try
            {
                Rocket.Core.Logging.Logger.Log("FLAG #5");
                //string responseText = await webClient.DownloadStringTaskAsync(UnloadLoaderRequest);
                responseText = await webClient.DownloadStringTaskAsync(GetUnloadLoaderRequest);
                Rocket.Core.Logging.Logger.Log("FLAG #6");
                RequestResponse response = null;
                Rocket.Core.Logging.Logger.Log("FLAG #7");

                Rocket.Core.Logging.Logger.Log("Response text: " + responseText);
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    Rocket.Core.Logging.Logger.Log("FLAG #8");

                    return new ServerResult(response);
                }
                Rocket.Core.Logging.Logger.Log("FLAG #10");
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
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            Rocket.Core.Logging.Logger.Log("FLAG #11");
            return null;
        }

        public async Task<ServerResult> GetLoaderEntryPointAsync(string key)
        {
            Rocket.Core.Logging.Logger.Log("FLAG #0.1");

            WebClient webClient = new WebClient();
            Rocket.Core.Logging.Logger.Log("FLAG #0.2");

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            Rocket.Core.Logging.Logger.Log("FLAG #0.3");

            webClient.Headers.Add("Key", key);
            Rocket.Core.Logging.Logger.Log("FLAG #0.4");

            string responseText = null;
            try
            {
                Rocket.Core.Logging.Logger.Log("FLAG #0.5");
                responseText = await webClient.DownloadStringTaskAsync(GetLoaderEntryPointRequest);
                Rocket.Core.Logging.Logger.Log("FLAG #0.6");

                Rocket.Core.Logging.Logger.Log("FLAG #0.7");
                Rocket.Core.Logging.Logger.Log("Response text in entry point: " + responseText);
                LoaderEntryPoint loaderEntryPoint = null;
                if ((loaderEntryPoint = JsonConvert.DeserializeObject<LoaderEntryPoint>(responseText)) != null)
                {
                    return new ServerResult(loaderEntryPoint);
                }

                Rocket.Core.Logging.Logger.Log("FLAG #0.9");
            }
            catch (JsonReaderException)
            {
                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        Rocket.Core.Logging.Logger.Log("FLAG #0.8");

                        return new ServerResult(response);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            Rocket.Core.Logging.Logger.Log("FLAG #0.10");
            return null;
        }

        public async Task<ServerResult> PutBlockPluginAsync(string key, string name)
        {
            Rocket.Core.Logging.Logger.Log("FLAG #0.1");

            WebClient webClient = new WebClient();
            Rocket.Core.Logging.Logger.Log("FLAG #0.2");

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            Rocket.Core.Logging.Logger.Log("FLAG #0.3");

            webClient.Headers.Add("Key", key);
            Rocket.Core.Logging.Logger.Log("FLAG #0.4");

            try
            {
                Rocket.Core.Logging.Logger.Log("FLAG #0.5");
                string responseText = await webClient.DownloadStringTaskAsync(string.Format(PutBlockPluginRequest, name));
                Rocket.Core.Logging.Logger.Log("FLAG #0.6");

                Rocket.Core.Logging.Logger.Log("FLAG #0.7");
                Rocket.Core.Logging.Logger.Log("Response text in entry point: " + responseText);
                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }

                Rocket.Core.Logging.Logger.Log("FLAG #0.9");
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            Rocket.Core.Logging.Logger.Log("FLAG #0.10");
            return null;
        }

        public async Task<ServerResult> PutUnblockPluginAsync(string key, string name)
        {
            Rocket.Core.Logging.Logger.Log("FLAG #0.1");

            WebClient webClient = new WebClient();
            Rocket.Core.Logging.Logger.Log("FLAG #0.2");

            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
            Rocket.Core.Logging.Logger.Log("FLAG #0.3");

            webClient.Headers.Add("Key", key);
            Rocket.Core.Logging.Logger.Log("FLAG #0.4");

            try
            {
                Rocket.Core.Logging.Logger.Log("FLAG #0.5");
                string responseText = await webClient.DownloadStringTaskAsync(string.Format(PutUnblockPluginRequest, name));
                Rocket.Core.Logging.Logger.Log("FLAG #0.6");

                Rocket.Core.Logging.Logger.Log("FLAG #0.7");
                Rocket.Core.Logging.Logger.Log("Response text in entry point: " + responseText);

                if (responseText != null)
                {
                    RequestResponse response = null;
                    if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                    {
                        return new ServerResult(response);
                    }
                }

                Rocket.Core.Logging.Logger.Log("FLAG #0.9");
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
                return new ServerResult(response.StatusCode);
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.Log($"An error occured while getting loader. {ex}");
            }
            Rocket.Core.Logging.Logger.Log("FLAG #0.10");
            return null;
        }
    }
}

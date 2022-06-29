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
        private const string UnloadLoaderRequest = "https://localhost:5001/bootstrapper/unloadLoader";
        private const string GetLoaderEntryPointRequest = "https://localhost:5001/bootstrapper/getLoaderEntryPoint";



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
                responseText = await webClient.DownloadStringTaskAsync(UnloadLoaderRequest);
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
                RequestResponse response = null;
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(JsonConvert.DeserializeObject<LoaderEntryPoint>(responseText));
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
    }
}

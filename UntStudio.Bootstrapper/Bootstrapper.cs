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
        //private const string GetUnloadLoaderRequest = "https://localhost:5001/bootstrapper/loadLoader";
        //private const string GetLoaderEntryPointRequest = "https://localhost:5001/bootstrapper/getloaderentrypoint";

        private const string LoadLoaderRequest = "https://untstudioserver20220710162140.azurewebsites.net/loadLoader";
        private const string GetLoaderEntryPointRequest = "https://untstudioserver20220710162140.azurewebsites.net/getloaderentrypoint";



        public async Task<ServerResult> UploadLoaderAsync(string licenseKey)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Bootstrapper");
            webClient.Headers.Add("LicenseKey", licenseKey);

            string responseText = null;
            try
            {
                responseText = await webClient.DownloadStringTaskAsync(LoadLoaderRequest);
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
                    Rocket.Core.Logging.Logger.LogWarning("License server is down, sorry about that.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Bootstrapper version is outdated.");
                }
                return new ServerResult(response.StatusCode);
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, $"An error occured while getting loader.");
            }
            return null;
        }

        public async Task<ServerResult> GetLoaderEntryPointAsync(string licenseKey)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Bootstrapper");
            webClient.Headers.Add("LicenseKey", licenseKey);

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
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Rocket.Core.Logging.Logger.LogWarning("License server is down, sorry about that.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Bootstrapper version is outdated.");
                }
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

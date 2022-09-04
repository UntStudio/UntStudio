using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.Models;

namespace UntStudio.Bootstrapper
{
    internal sealed class Bootstrapper : IBootstrapper
    {
        public async Task<ServerResult> UploadLoaderAsync(string licenseKey)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Bootstrapper");
            webClient.Headers.Add("LicenseKey", licenseKey);

            string responseText = null;
            try
            {
                responseText = await webClient.DownloadStringTaskAsync(new Uri("http://135.181.47.150/bootstrapper/loadLoader"));
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
            catch (SocketException)
            {
                Rocket.Core.Logging.Logger.LogWarning("Looks like license server is down or couldn`t connect to license server. Please, check your internet connection and firewall rules.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, $"An unknown error occured while getting loader.");
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
                responseText = await webClient.DownloadStringTaskAsync(new Uri("http://135.181.47.150/bootstrapper/getloaderentrypoint"));
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
            catch (SocketException)
            {
                Rocket.Core.Logging.Logger.LogWarning("Looks like license server is down or couldn`t connect to license server. Please, check your internet connection and firewall rules.");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An unknown error occured while getting loader.");
            }
            return null;
        }
    }
}

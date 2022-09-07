using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.API;
using UntStudio.Loader.API.Models;
using UntStudio.Loader.API.Services;

namespace UntStudio.Loader.Servers;

public sealed class Server : IServer
{
    private readonly ILogging logging;



    public Server(ILogging logging)
    {
        this.logging = logging;
    }



    public async Task<ServerResult> UploadPluginAsync(string licenseKey, string name, CancellationToken cancellationToken = default)
    {
        WebClient webClient = new WebClient();
        webClient.Headers.Add(HeaderNames.UserAgent, "UntStudio.Loader");
        webClient.Headers.Add("LicenseKey", licenseKey);

        string responseText = null;
        try
        {
            responseText = await webClient.DownloadStringTaskAsync(new Uri($"http://135.181.47.150/pluginSubscriptions/load?name={name}"));
            RequestResponse response = null;
            if (string.IsNullOrWhiteSpace(responseText) == false)
            {
                if ((response = JsonConvert.DeserializeObject<RequestResponse>(responseText)) != null)
                {
                    return new ServerResult(response);
                }
            }
        }
        catch (JsonReaderException)
        {
            if (string.IsNullOrWhiteSpace(responseText) == false)
            {
                return new ServerResult(Convert.FromBase64String(responseText));
            }
        }
        catch (WebException ex) when (ex.Response is HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                this.logging.LogWarning("License server is down, sorry about that.");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                this.logging.LogWarning("Loader version is outdated.");
            }

            return new ServerResult(response.StatusCode);
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ConnectFailure)
            {
                this.logging.LogWarning("Connection failure, server is down.");
            }
            else
            {
                this.logging.LogWarning("Couldn`t connect to license server. Please, check your internet connection and firewall rules.");
            }
        }
        catch (SocketException)
        {
            this.logging.LogWarning("Looks like license server is down or couldn`t connect to license server. Please, check your internet connection and firewall rules.");
        }
        catch (Exception ex)
        {
            this.logging.LogException(ex, "An error occured while getting plugin!");
        }
        return new ServerResult();
    }
}

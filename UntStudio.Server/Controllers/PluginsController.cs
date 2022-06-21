using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using static UntStudio.Server.Models.PluginRequestResult;

namespace UntStudio.Server.Controllers
{
    public class PluginsController : Controller
    {
        private readonly PluginsDatabaseContext database;

        private readonly IConfiguration configuration;



        public PluginsController(PluginsDatabaseContext database, IConfiguration configuration)
        {
            this.database = database;
            this.configuration = configuration;
        }



        public IActionResult GetPlugin(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest();
            }

            if (key.Length != 19)
            {
                return BadRequest();
            }

            Plugin plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key));
            if (plugin == null)
            {
                return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.NotFound)));
            }

            if (plugin.Expired)
            {
                return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.SubscriptionExpired)));
            }

            return Ok(JsonConvert.SerializeObject(plugin));
        }

        public IActionResult Unload(string key, string pluginName)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(pluginName))
            {
                return BadRequest();
            }

            Plugin plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(pluginName));
            if (plugin == null)
            {
                return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.NotFound)));
            }

            if (plugin.Expired)
            {
                return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.SubscriptionExpired)));
            }

            string path = Path.Combine(this.configuration["PluginsDirectory:Path"], pluginName + ".dll");
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(path)));
        }
    }
}

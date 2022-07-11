using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UntStudio.Server.Data;

namespace UntStudio.Server.Controllers
{
    [Authorize]
    public sealed class AdminsMenuController : Controller
    {
        private readonly PluginSubscriptionsDatabaseContext database;



        public AdminsMenuController(PluginSubscriptionsDatabaseContext database)
        {
            this.database = database;
        }



        public IActionResult HandleButtonClick()
        {
            return Ok();
        }
    }
}

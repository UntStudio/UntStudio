using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UntStudio.Server.Data;

namespace UntStudio.Server.Controllers;

public sealed class AdminsAuthorization : Controller
{
    private readonly AdminsDatabaseContext database;



    public AdminsAuthorization(AdminsDatabaseContext database)
    {
        this.database = database;
    }



    [HttpGet("login")]
    public IActionResult Login(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Validate(string login, string password, string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (this.database.Data.ToList().FirstOrDefault(a => a.Login.Equals(login) && a.Password.Equals(password)) == null)
        {
            TempData["ErrorMessage"] = "Login or password is invalid";
            return RedirectToAction(nameof(Login));
        }

        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, login));
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);
        return Redirect(returnUrl ?? "/profile/");
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}

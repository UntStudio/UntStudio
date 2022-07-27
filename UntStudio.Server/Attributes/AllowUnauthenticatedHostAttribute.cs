using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using UntStudio.Server.Knowns;

namespace UntStudio.Server.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class AllowUnauthenticatedHostAttribute : TypeFilterAttribute
{
    public AllowUnauthenticatedHostAttribute(string host) : base(typeof(AllowUnauthenticatedHostAuthorizationFilter))
    {
        Arguments = new object[]
        {
            host,
        };
    }
}

public sealed class AllowUnauthenticatedHostAuthorizationFilter : IAuthorizationFilter
{
    private readonly string host;



    public AllowUnauthenticatedHostAuthorizationFilter(string host)
    {
        this.host = host;
    }



    public void OnAuthorization(AuthorizationFilterContext context)
    {
        Console.WriteLine(context.HttpContext.Request.Host);
        if (context.HttpContext.User.Identity.IsAuthenticated == false && context.HttpContext.Request.Host.Host != host)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}

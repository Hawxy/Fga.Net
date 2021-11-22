using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Sandcastle.AspNetCore.Authorization;

internal class SandcastleAuthorizationHandler: IAuthorizationHandler
{
    private readonly IHttpContextAccessor _accessor;

    public SandcastleAuthorizationPolicy(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
       var routeData =  _accessor.HttpContext!.GetRouteData();

    }
}

public class SandcastleAuthorizeAttribute : AuthorizeAttribute
{
    private const string POLICY_PREFIX = "Sandcastle";


}
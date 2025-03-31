using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Example.AspNetCore.TestControllers;

[ApiController]
[Route("[controller]")]
[Authorize(FgaAuthorizationDefaults.PolicyKey)]
[TestAuthorization]
public class TestController : ControllerBase
{

    [HttpGet("document/{documentId}")]
    public string GetByConvention(string documentId)
    {
        return documentId;
    }
    
    [HttpGet("ignored")]
    [FgaBypass]
    public string GetIgnored()
    {
        return string.Empty;
    }


}
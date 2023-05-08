using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Example.AspNetCore.TestControllers;

[ApiController]
[Route("[controller]")]
[Authorize(FgaAuthorizationDefaults.PolicyKey)]
public class TestController : ControllerBase
{

    [HttpGet("{documentId}")]
    [TestAuthorization]
    public string GetByConvention(string documentId)
    {
        return documentId;
    }


}
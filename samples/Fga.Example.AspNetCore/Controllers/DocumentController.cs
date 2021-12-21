using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Example.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(FgaAuthorizationDefaults.PolicyKey)]
    public class DocumentController : ControllerBase
    {

        [HttpGet]
        [StringComputedAuthorization("anne", "read", "doc:Z")]
        public string GetHardcoded()
        {
            return string.Empty;
        }

        [HttpGet("view/{documentId}")]
        [EntityAuthorization("doc", "documentId")]
        public string GetByConvention(string documentId)
        {
            return documentId;
        }
    }
}
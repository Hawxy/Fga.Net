using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(FgaAuthorizationDefaults.PolicyKey)]
    public class DocumentController : ControllerBase
    {

        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [StringComputedAuthorization("anne", "read", "doc:Z")]
        public string Get(string documentId)
        {
            return string.Empty;
        }
    }
}
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Example.AspNetCore.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(FgaAuthorizationDefaults.PolicyKey)]
public class DocumentController : ControllerBase
{

    [HttpGet("view/{documentId}")]
    [FgaRouteObject(Fga.Document.Read, Fga.Document.Type, nameof(documentId))]
    public string GetByConvention(string documentId)
    {
        return documentId;
    }

    [HttpGet]
    [FgaString("anne", "read", "document:12345")]
    public string GetHardcoded()
    {
        return string.Empty;
    }

}

public static class Fga
{
    public class Document
    {
        public const string Type = "document";

        public const string Owner = "owner";
        public const string Read = "reader";
        public const string Write = "writer";

    }
}
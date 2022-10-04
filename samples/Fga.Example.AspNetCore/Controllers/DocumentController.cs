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
    [FgaRouteObject(Fga.Document.Viewer, Fga.Document.Type, "documentId")]
    public string GetByConvention(string documentId)
    {
        return documentId;
    }

    [HttpGet]
    [FgaString("anne", "viewer", "document:12345")]
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

        public const string Viewer = "viewer";
        public const string Editor = "editor";
        public const string Owner = "owner";

    }
}
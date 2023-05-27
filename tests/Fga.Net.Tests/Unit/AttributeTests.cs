using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.AspNetCore.Exceptions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Fga.Net.Tests.Unit;

public class AttributeTests
{
    [Fact]
    public async Task HeaderObjectAttribute_WorksAsExpected()
    {
        var httpContext = new HttpContextMock()
            .SetupRequestHeaders(new Dictionary<string, StringValues>() {{"documentId", "12345"}});

        var attribute = new FgaHeaderObjectAttribute("relation", "type", "documentId");

        var obj = await attribute.GetObject(httpContext);

        Assert.Equal("type:12345", obj);
    }

    [Fact]
    public async Task HeaderObjectAttribute_ThrowsOnMissingHeader()
    {
        var httpContext = new HttpContextMock()
            .SetupRequestHeaders(new Dictionary<string, StringValues>() { { "documentId", "12345" } });

        var attribute = new FgaHeaderObjectAttribute("relation", "type", "randomHeader");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));
        Assert.Equal("Header randomHeader was not present in the request", exception.Message);
    }

    [Fact]
    public async Task PropertyObjectAttribute_WorksAsExpected()
    {
        var data = Encoding.UTF8.GetBytes("{\"Foo\":\"Bar\",\"Array\":[]}");

        var httpContext = new HttpContextMock()
                .SetupUrl("http://localhost:8000/path")
                .SetupRequestMethod("POST")
                .SetupRequestContentType("application/json")
                .SetupRequestBody(new MemoryStream(data))
                .SetupRequestContentLength(data.Length);

        var attribute = new FgaPropertyObjectAttribute("relation", "type", "Foo");

        var obj = await attribute.GetObject(httpContext);

        Assert.Equal("type:Bar", obj);
    }

    [Fact]
    public async Task PropertyObjectAttribute_ThrowsOnMissingProperty()
    {
        var data = "{\"Foo\":\"Bar\",\"Array\":[]}"u8.ToArray();

        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/path")
            .SetupRequestMethod("POST")
            .SetupRequestContentType("application/json")
            .SetupRequestBody(new MemoryStream(data))
            .SetupRequestContentLength(data.Length);

        var attribute = new FgaPropertyObjectAttribute("relation", "type", "RandomProperty");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext)); 
        
        Assert.Equal("Unable to resolve JSON property RandomProperty", exception.Message);
    }
    
    [Fact]
    public async Task QueryObjectAttribute_WorksAsExpected()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/path?documentId=12345&otherQueryString=55555");

        var attribute = new FgaQueryObjectAttribute("relation", "type", "documentId");

        var obj = await attribute.GetObject(httpContext);

        Assert.Equal("type:12345", obj);
    }
    
    [Fact]
    public async Task QueryObjectAttribute_ThrowsOnMissingQueryKey()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/path?documentId=12345&otherQueryString=55555");

        var attribute = new FgaQueryObjectAttribute("relation", "type", "randomQueryKey");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));
        Assert.Equal("Query key randomQueryKey was not present in the request", exception.Message);
    }
    
    [Fact]
    public async Task RouteObjectAttribute_WorksAsExpected()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/12345/");
        
        httpContext.FeaturesMock.Mock.Setup<IRouteValuesFeature>(x => x.Get<IRouteValuesFeature>()!).Returns(new RouteValuesFeature()
        {
           RouteValues = {{"documentId", 12345}}
        });

        var attribute = new FgaRouteObjectAttribute("relation", "type", "documentId");

        var obj = await attribute.GetObject(httpContext);

        Assert.Equal("type:12345", obj);
    }

    [Fact]
    public async Task RouteObjectAttribute_ThrowsOnMissingRouteValue()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/12345/");

        var attribute = new FgaRouteObjectAttribute("relation", "type", "randomKey");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));

        Assert.Equal("Route value randomKey was not present in the request", exception.Message);
    }

}
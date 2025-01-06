using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.AspNetCore.Exceptions;
using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Fga.Net.Tests.Unit;

public class AttributeTests
{
    [Test]
    public async Task HeaderObjectAttribute_WorksAsExpected()
    {
        var httpContext = new HttpContextMock()
            .SetupRequestHeaders(new Dictionary<string, StringValues>() {{"documentId", "12345"}});

        var attribute = new FgaHeaderObjectAttribute("relation", "type", "documentId");

        var obj = await attribute.GetObject(httpContext);

        obj.Should().Be("type:12345");
    }

    [Test]
    public async Task HeaderObjectAttribute_ThrowsOnMissingHeader()
    {
        var httpContext = new HttpContextMock()
            .SetupRequestHeaders(new Dictionary<string, StringValues>() { { "documentId", "12345" } });

        var attribute = new FgaHeaderObjectAttribute("relation", "type", "randomHeader");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));

        exception.Message.Should().Be("Header randomHeader was not present in the request");
    }

    [Test]
    public async Task PropertyObjectAttribute_WorksAsExpected()
    {
        var data = "{\"Foo\":\"Bar\",\"Array\":[]}"u8.ToArray();

        var httpContext = new HttpContextMock()
                .SetupUrl("http://localhost:8000/path")
                .SetupRequestMethod("POST")
                .SetupRequestContentType("application/json")
                .SetupRequestBody(new MemoryStream(data))
                .SetupRequestContentLength(data.Length);

        var attribute = new FgaPropertyObjectAttribute("relation", "type", "Foo");

        var obj = await attribute.GetObject(httpContext);

        obj.Should().Be("type:Bar");
    }

    [Test]
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

        exception.Message.Should().Be("Unable to resolve JSON property RandomProperty");
    }
    
    [Test]
    public async Task QueryObjectAttribute_WorksAsExpected()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/path?documentId=12345&otherQueryString=55555");

        var attribute = new FgaQueryObjectAttribute("relation", "type", "documentId");

        var obj = await attribute.GetObject(httpContext);
        
        obj.Should().Be("type:12345");
    }
    
    [Test]
    public async Task QueryObjectAttribute_ThrowsOnMissingQueryKey()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/path?documentId=12345&otherQueryString=55555");

        var attribute = new FgaQueryObjectAttribute("relation", "type", "randomQueryKey");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));
        
        exception.Message.Should().Be("Query key randomQueryKey was not present in the request");
    }
    
    [Test]
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

        obj.Should().Be("type:12345");
    }

    [Test]
    public async Task RouteObjectAttribute_ThrowsOnMissingRouteValue()
    {
        var httpContext = new HttpContextMock()
            .SetupUrl("http://localhost:8000/12345/");

        var attribute = new FgaRouteObjectAttribute("relation", "type", "randomKey");

        var exception = await Assert.ThrowsAsync<FgaMiddlewareException>(async () => await attribute.GetObject(httpContext));

        exception.Message.Should().Be("Route value randomKey was not present in the request");
    }

}
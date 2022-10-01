using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Example.AspNetCore;

//Checks against a value in the route.
public class EntityAuthorizationAttribute : FgaBaseAttribute
{
    private readonly string _type;
    private readonly string _routeValue;
    public EntityAuthorizationAttribute(string type, string routeValue)
    {
        _type = type;
        _routeValue = routeValue;
    }

    public override ValueTask<string> GetUser(HttpContext context) 
        => ValueTask.FromResult(context.User.Identity!.Name!);

    public override ValueTask<string> GetRelation(HttpContext context) 
        => ValueTask.FromResult(context.Request.Method switch 
        {
            "GET" => "viewer",
            "POST" => "writer",
            _ => "owner"
        });

    public override ValueTask<string> GetObject(HttpContext context) 
        => ValueTask.FromResult(FormatObject(_type, context.GetRouteValue(_routeValue)!.ToString()!));
}

// Checks against a single entity sharing a common interface.
public class InterfaceAuthorizationAttribute : FgaBaseAttribute
{
    private readonly string _prefix;
    public InterfaceAuthorizationAttribute(string prefix)
    {
        _prefix = prefix;
    }


    public override ValueTask<string> GetUser(HttpContext context)
        => ValueTask.FromResult(context.User.Identity!.Name!);

    public override ValueTask<string> GetRelation(HttpContext context)
        => ValueTask.FromResult(context.Request.Method switch
        {
            "GET" => "viewer",
            "POST" => "writer",
            _ => "owner"
        });

    public override async ValueTask<string> GetObject(HttpContext context)
    {
        context.Request.EnableBuffering();
        var entity = await context.Request.ReadFromJsonAsync<IEntity>();
        return entity is not null ? $"{_prefix}:{entity.Id}" : string.Empty;
    }
}

public interface IEntity
{
    public Guid Id { get; set; }
}

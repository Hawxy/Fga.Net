using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Example.AspNetCore;

//Computes the relationship based on the requests HTTP method.
public class ComputedRelationshipAttribute : FgaBaseObjectAttribute
{
    private readonly string _type;
    private readonly string _routeValue;
    public ComputedRelationshipAttribute(string type, string routeValue)
    {
        _type = type;
        _routeValue = routeValue;
    }

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
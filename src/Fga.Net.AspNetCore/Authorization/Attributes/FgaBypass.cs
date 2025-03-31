namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Bypasses the FGA Authorization middleware by forcing it to succeed.
/// Prefer the usage of Policy construction over this attribute where possible. 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FgaBypass : Attribute
{
    
}
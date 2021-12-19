namespace Fga.Net;

public static class FgaUtilities
{
    public static Uri GetAuthorizationUri(string environment) => new(string.Format(FgaConstants.AuthorizationUrl, environment));

}
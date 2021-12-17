namespace Fga.Net;

public class FgaClientConfiguration
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;

    /// <summary>
    /// Defaults to US if not set
    /// </summary>
    public string Environment { get; set; } = "us1";
    public string StoreId { get; set; } = null!;
}
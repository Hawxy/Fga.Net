namespace Sandcastle;
public class SandcastleClient
{
    private readonly HttpClient _client;
    private readonly SandcastleClientConfiguration _configuration;

    public SandcastleClient(SandcastleClientConfiguration configuration, HttpClient client)
    {
        _client = client;
        _configuration = configuration;
    }



    public async Task FetchTokenAsync(AccessTokenRequest request)
    {

    }

}

public class SandcastleClientConfiguration
{
    
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Environment { get; set; }
}

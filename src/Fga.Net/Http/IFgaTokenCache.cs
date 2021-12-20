namespace Fga.Net.Http;

internal interface IFgaTokenCache
{
    Task<string> GetOrUpdateTokenAsync();
}
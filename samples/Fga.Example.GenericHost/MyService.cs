using Fga.Net.Authorization;

namespace Fga.Example.GenericHost
{
    public class MyService
    {
        private readonly IFgaAuthorizationClient _authorizationClient;

        public MyService(IFgaAuthorizationClient authorizationClient)
        {
            _authorizationClient = authorizationClient;
        }
    }
}

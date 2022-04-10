using Auth0.Fga.Api;

namespace Fga.Example.GenericHost
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly Auth0FgaApi _authorizationClient;

        public MyBackgroundWorker(Auth0FgaApi authorizationClient)
        {
            _authorizationClient = authorizationClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Do work with the client
           return Task.CompletedTask;
        }
    }
}

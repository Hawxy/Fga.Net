using OpenFga.Sdk.Api;

namespace Fga.Example.GenericHost
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly OpenFgaApi _authorizationClient;

        public MyBackgroundWorker(OpenFgaApi authorizationClient)
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

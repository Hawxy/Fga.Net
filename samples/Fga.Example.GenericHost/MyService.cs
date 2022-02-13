using Fga.Net.Authorization;

namespace Fga.Example.GenericHost
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly IFgaAuthorizationClient _authorizationClient;

        public MyBackgroundWorker(IFgaAuthorizationClient authorizationClient)
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

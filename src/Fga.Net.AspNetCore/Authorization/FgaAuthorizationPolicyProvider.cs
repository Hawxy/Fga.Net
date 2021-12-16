namespace Fga.Net.AspNetCore.Authorization
{
    // Alternative design. Only revisit if we need to handle different sandcastle implementations via multiple policies.
    /*
    internal class SandcastleAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public SandcastleAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {


            if (FgaCheck.TryParse(policyName, out var fgaCheck))
            {
                // spin up authorization policy
            }

            return BackupPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();
    }*/
}

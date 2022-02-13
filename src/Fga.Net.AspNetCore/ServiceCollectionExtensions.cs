#region License
/*
   Copyright 2021-2022 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using Fga.Net.AspNetCore.Authorization;
using Fga.Net.Authentication;
using Fga.Net.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fga.Net.AspNetCore;

/// <summary>
/// Extensions for registering Fga.Net.AspNetCore features to an ASP.NET Core environment.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures an <see cref="FineGrainedAuthorizationHandler"/> along with a <see cref="FgaAuthenticationClient"/> and <see cref="FgaAuthorizationClient"/>
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The delegate for the <see cref="FgaClientConfiguration"/> that will be used to configure the <see cref="FgaAuthorizationClient"/></param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuth0Fga(this IServiceCollection collection, Action<FgaClientConfiguration> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        collection.AddAuth0FgaAuthenticationClient();
        collection.AddAuth0FgaAuthorizationClient(config);
        collection.AddScoped<IAuthorizationHandler, FineGrainedAuthorizationHandler>();
        return collection;
    }

    /// <summary>
    /// Adds a <see cref="FineGrainedAuthorizationRequirement"/> to the given policy.
    /// </summary>
    /// <param name="builder">The Authorization Policy Builder to configure</param>
    /// <param name="storeId">The store ID this requirement should check against</param>
    /// <returns>The authorization policy builder that is being configured</returns>
    public static AuthorizationPolicyBuilder AddFgaRequirement(this AuthorizationPolicyBuilder builder, string storeId)
    {
        ArgumentNullException.ThrowIfNull(storeId);

        return builder.AddRequirements(new FineGrainedAuthorizationRequirement(storeId));
    }
}

#region License
/*
   Copyright 2021-2024 Hawxy (JT)

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


using System.Security.Claims;
using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Net.AspNetCore;

/// <summary>
/// Configuration for FGAs Middleware
/// </summary>
public sealed class FgaAspNetCoreConfiguration
{
    internal Func<ClaimsPrincipal, string>? UserIdentityResolver { get; private set; }

    /// <summary>
    /// Configures the user identifier to be used for built-in check requests.
    /// <example>
    ///  SetUserIdentifier("user", principal => principal.Identity!.Name!);
    /// </example>
    /// 
    /// Used by all attributes derived from <see cref="FgaBaseObjectAttribute"/>.
    /// </summary>
    /// <param name="type">The user type within your FGA model, such as user.</param>
    /// <param name="idResolver">A resolver that gets the UserId from the requests claims.</param>
    public void SetUserIdentifier(string type, Func<ClaimsPrincipal, string> idResolver)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(idResolver);

        UserIdentityResolver = principal => $"{type}:{idResolver(principal)}";
    }
}
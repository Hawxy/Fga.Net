#region License
/*
   Copyright 2021-2025 Hawxy

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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Base type for implementing attributes that use a configuration-driven user source.
/// </summary>
public abstract class FgaBaseObjectAttribute : FgaAttribute
{
    /// <inheritdoc />
    public override ValueTask<string> GetUser(HttpContext context)
    {
        var config = context.RequestServices.GetRequiredService<IOptions<FgaAspNetCoreConfiguration>>().Value;
        return ValueTask.FromResult(config.UserIdentityResolver!(context.User));
    }
}
#region License
/*
   Copyright 2021-2025 Hawxy (JT)

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
using Fga.Net.DependencyInjection.Configuration;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;

namespace Fga.Net.DependencyInjection;

internal sealed class InjectableFgaApi : OpenFgaApi
{
    public InjectableFgaApi(IOptions<FgaClientConfiguration> configuration, IHttpClientFactory httpClientFactory) : base(configuration.Value.PurgeHeader(), httpClientFactory.CreateClient(Constants.FgaHttpClient))
    {
    }
}

internal sealed class InjectableFgaClient : OpenFgaClient
{
    public InjectableFgaClient(IOptions<FgaClientConfiguration> configuration, IHttpClientFactory httpClientFactory) : base(configuration.Value.PurgeHeader(), httpClientFactory.CreateClient(Constants.FgaHttpClient))
    {
    }
}

internal static class ConfigurationExtensions
{
    // This fixes an exception when using API Tokens and both clients.
    public static FgaClientConfiguration PurgeHeader(this FgaClientConfiguration configuration)
    {
        configuration.DefaultHeaders.Remove("Authorization");
        return configuration;
    }
}
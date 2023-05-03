#region License
/*
   Copyright 2021-2023 Hawxy

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
using OpenFga.Sdk.Client;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Base configuration for all Fga implementations
/// </summary>
public sealed class FgaConfigurationRoot 
{
    /// <inheritdoc cref="ClientConfiguration.StoreId"/>
    public string? StoreId { get; set; }

    /// <inheritdoc cref="ClientConfiguration.AuthorizationModelId"/>
    public string? AuthorizationModelId { get; set; }

    /// <inheritdoc cref="ClientConfiguration.MaxRetry"/>
    public int? MaxRetry { get; set; }

    /// <inheritdoc cref="ClientConfiguration.MinWaitInMs"/>
    public int? MinWaitInMs { get; set; }

    private FgaConnectionConfiguration? _fgaConfiguration;

    /// <summary>
    /// Configures the client for use with OpenFga
    /// </summary>
    /// <param name="config"></param>
    public void ConfigureOpenFga(Action<OpenFgaConnectionBuilder> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        
        var configuration = new OpenFgaConnectionBuilder();
        config.Invoke(configuration);
        _fgaConfiguration = configuration.Build();
    }

    
    /// <summary>
    /// Configures the client for use with Auth0 Fga
    /// </summary>
    /// <param name="config"></param>
    public void ConfigureAuth0Fga(Action<Auth0FgaConnectionBuilder> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        
        var configuration = new Auth0FgaConnectionBuilder();
        config.Invoke(configuration);
        _fgaConfiguration = configuration.Build();
    }

    internal FgaConnectionConfiguration GetConnectionConfiguration()
    {
        if (_fgaConfiguration is null)
            throw new InvalidOperationException("OpenFga or Auth0 FGA configuration must be added");
        return _fgaConfiguration;
    }
}
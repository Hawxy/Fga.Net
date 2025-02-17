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
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Base configuration for all Fga implementations
/// </summary>
public sealed class FgaConfigurationBuilder 
{
    private FgaConnectionConfiguration? _fgaConfiguration;

    private string? _storeId;
    
    /// <inheritdoc cref="ClientConfiguration.StoreId"/>
    public FgaConfigurationBuilder SetStoreId(string storeId)
    {
        _storeId = storeId;
        return this;
    }

    private string? _authorizationModelId;
    
    /// <inheritdoc cref="ClientConfiguration.AuthorizationModelId"/>
    public FgaConfigurationBuilder SetAuthorizationModelId(string authorizationModelId)
    {
        _authorizationModelId = authorizationModelId;
        return this;
    }

    private int? _maxRetry;

    /// <inheritdoc cref="ClientConfiguration.MaxRetry"/>
    public FgaConfigurationBuilder SetMaxRetry(int maxRetry)
    {
        _maxRetry = maxRetry;
        return this;
    }

    private int? _minWaitInMs;
    /// <inheritdoc cref="ClientConfiguration.MinWaitInMs"/>
    public FgaConfigurationBuilder SetWaitInMs(int waitInMs)
    {
        _minWaitInMs = waitInMs;
        return this;
    }

    private TelemetryConfig? _telemetryConfig;
    
    /// <inheritdoc cref="ClientConfiguration.Telemetry"/>
    public FgaConfigurationBuilder SetTelemetry(TelemetryConfig config)
    {
        _telemetryConfig = config;
        return this;
    }

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
    [Obsolete("Replace with ConfigureOktaFga. This will be removed in the next major release.")]
    public void ConfigureAuth0Fga(Action<OktaFgaConnectionBuilder> config)
    {
        ConfigureOktaFga(config);
    }
    
    /// <summary>
    /// Configures the client for use with Okta Fga
    /// </summary>
    /// <param name="config"></param>
    public void ConfigureOktaFga(Action<OktaFgaConnectionBuilder> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        
        var configuration = new OktaFgaConnectionBuilder();
        config.Invoke(configuration);
        _fgaConfiguration = configuration.Build();
    }

    
    internal FgaBuiltConfiguration Build()
    {
        if (_fgaConfiguration is null)
            throw new InvalidOperationException("OpenFga or Auth0 FGA configuration must be set");
        if (string.IsNullOrEmpty(_storeId))
            throw new InvalidOperationException("Store ID must be set");

        return new FgaBuiltConfiguration(_storeId, _authorizationModelId, _maxRetry, _minWaitInMs, _telemetryConfig, _fgaConfiguration);
    }
}
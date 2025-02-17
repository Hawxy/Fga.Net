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

using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;

namespace Fga.Net.AspNetCore.Authorization;

/// <summary>
/// Temporary wrapper to allow for easier testing of middleware. Don't take a dependency on this.
/// </summary>
public class FgaCheckDecorator : IFgaCheckDecorator
{
    private readonly OpenFgaClient _auth0FgaApi;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auth0FgaApi"></param>
    public FgaCheckDecorator(OpenFgaClient auth0FgaApi)
    {
        _auth0FgaApi = auth0FgaApi;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public virtual Task<BatchCheckResponse> BatchCheck(List<ClientCheckRequest> request, CancellationToken ct) => _auth0FgaApi.BatchCheck(request, cancellationToken: ct);
}

/// <summary>
/// Temporary wrapper to allow for easier testing of middleware. Don't take a dependency on this.
/// </summary>
public interface IFgaCheckDecorator
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<BatchCheckResponse> BatchCheck(List<ClientCheckRequest> request, CancellationToken ct);
}
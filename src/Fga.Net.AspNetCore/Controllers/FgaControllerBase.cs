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

using Microsoft.AspNetCore.Mvc;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Model;

namespace Fga.Net.AspNetCore.Controllers;

/// <summary>
/// A base ASP.NET Core controller that adds an extension method for FGA checks.
/// </summary>
public class FgaControllerBase : ControllerBase
{
    private readonly OpenFgaApi _client;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public FgaControllerBase(OpenFgaApi client)
    {
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="relation"></param>
    /// <param name="object"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> Check(string user, string relation, string @object, CancellationToken ct)
    {
        var checkRes = await _client.Check(new CheckRequest()
        {
            TupleKey = new TupleKey
            {
                User = user,
                Relation = relation,
                Object = @object
            }
        }, ct);
        return checkRes.Allowed;
    }
}
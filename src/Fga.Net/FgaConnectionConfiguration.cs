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

using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;


internal sealed record FgaBuiltConfiguration(
    string StoreId, 
    string? AuthorizationModelId, 
    int? MaxRetry,
    int? MinWaitInMs,
    FgaConnectionConfiguration Connection);

internal sealed record FgaConnectionConfiguration(string ApiScheme, string ApiHost, Credentials? Credentials);
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

namespace Fga.Net;

/// <summary>
/// FGA Authentication/Authorization configuration
/// </summary>
public class FgaClientConfiguration
{
    /// <summary>
    /// The Client Id for your FGA instance
    /// </summary>
    public string ClientId { get; set; } = null!;
    /// <summary>
    /// The Client Secret for your FGA instance
    /// </summary>
    public string ClientSecret { get; set; } = null!;

    /// <summary>
    /// Defaults to us1 if not set
    /// </summary>
    public string Environment { get; set; } = "us1";
}
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
/// Static FGA utilities
/// </summary>
public static class FgaUtilities
{
    /// <summary>
    /// Creates a FGA authorization URL for the specified environment
    /// </summary>
    /// <param name="environment">The environment, such as us1</param>
    /// <returns>A URL</returns>
    public static string GetAuthorizationUrl(string environment) => $"https://api.{environment}.fga.dev/";
    /// <summary>
    /// Creates a FGA audience URL for the specified environment.
    /// </summary>
    /// <param name="environment">The environment, such as us1</param>
    /// <returns>A URL</returns>
    public static string GetAudienceUrl(string environment) => $"https://api.{environment}.fga.dev/";

}
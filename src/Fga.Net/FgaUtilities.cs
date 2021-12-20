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

using System.Net;
using System.Text;

namespace Fga.Net;

/// <summary>
/// Static FGA utilities
/// </summary>
public static class FgaUtilities
{
    /// <summary>
    /// Creates a new FGA Authorization <see cref="Uri"/> based on the provided environment
    /// </summary>
    /// <param name="environment">The environment, such as "us1"</param>
    /// <returns></returns>
    public static Uri GetAuthorizationUri(string environment) => new(string.Format(FgaConstants.AuthorizationUrl, environment));

    internal static string BuildQueryString(this string request, params (string parameter, string? value)[] queryStrings)
    {
        var validQueryStrings = queryStrings.Where(x => x.value is not null).ToList();

        if (!validQueryStrings.Any())
            return request;
      
        var builder = new StringBuilder(request);
        builder.Append('?');
        for (var i = 0; i < validQueryStrings.Count; i++)
        {
            var (parameter, value) = validQueryStrings[i];

            if (i > 0)
                builder.Append('&');

            builder.Append($"{WebUtility.UrlEncode(parameter)}={WebUtility.UrlEncode(value)}");
        }

        return builder.ToString();
    }

}
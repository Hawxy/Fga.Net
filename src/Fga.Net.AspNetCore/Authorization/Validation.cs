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
namespace Fga.Net.AspNetCore.Authorization;

internal static class Validation
{
    // Validate the user is either in the type:id format or '*'
    public static bool IsValidUser(string user)
    {
        var strSpan = user.AsSpan();

        var seperatorCount = 0;

        for (var i = 0; i < strSpan.Length; i++)
        {
            var c = strSpan[i];
            // if the string contains whitespace then it isn't valid.
            if (char.IsWhiteSpace(c))
                return false;

            if (c == ':')
            {
                // if : is at the start or end it isn't valid
                if (i == 0 || i == strSpan.Length - 1)
                    return false;
                seperatorCount++;
            }
        
        }

        if (seperatorCount == 1)
            return true;

        if (strSpan.Length == 1 && strSpan[0] == '*')
            return true;

        return false;
    }   
}
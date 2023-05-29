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
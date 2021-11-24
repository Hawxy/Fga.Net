using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Sandcastle.AspNetCore.Authorization;

// TODO delete this?

/// <summary>
/// A record that represents a Sandcastle Check operation
/// </summary>
/// <param name="Object">The object to check against. Examples: `document:id, feature:accounting`</param>
/// <param name="Relation">The relation to check, such as manager, writer, or reader.</param>
/// <param name="User">The user you're checking against, such as an email or Auth0 Id</param>
public record FgaCheck(string Object, string Relation, string User)
{

    private const char Separator = '-';
    public override string ToString() => $"{SandcastleAuthorizationDefaults.PolicyKey}-{Object}-{Relation}-{User}";

    public static bool TryParse(string input, [NotNullWhen(true)] out FgaCheck? fgaCheck)
    {
        var str = input.Split(Separator, 4);
        if (str.Length == 4 && str[0].Equals(SandcastleAuthorizationDefaults.PolicyKey))
        {
            fgaCheck = new FgaCheck(str[1], str[2], str[3]);
            return true;
        }

        fgaCheck = null;
        return false;
    }

}

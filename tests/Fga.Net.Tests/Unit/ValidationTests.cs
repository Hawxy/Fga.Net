using Fga.Net.AspNetCore.Authorization;
using FluentAssertions;

namespace Fga.Net.Tests.Unit;

public class ValidationTests
{
    [Test]
    [Arguments("*", true)]
    [Arguments("type:id", true)]
    [Arguments("*asdf", false)]
    [Arguments("type:", false)]
    [Arguments(":user", false)]
    [Arguments(":", false)]
    public void UserValidation_ValidatesCorrectly(string user, bool expected)
    {
        Validation.IsValidUser(user).Should().Be(expected);
    }
}

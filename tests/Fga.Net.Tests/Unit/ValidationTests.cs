using Fga.Net.AspNetCore.Authorization;
using Xunit;

namespace Fga.Net.Tests.Unit;

public class ValidationTests
{
    [Theory]
    [InlineData("*", true)]
    [InlineData("type:id", true)]
    [InlineData("*asdf", false)]
    [InlineData("type:", false)]
    [InlineData(":", false)]
    public void UserValidation_ValidatesCorrectly(string user, bool expected)
    {
        Assert.Equal(expected, Validation.IsValidUser(user)); 
    }
}

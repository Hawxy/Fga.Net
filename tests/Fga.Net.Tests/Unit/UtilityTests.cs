using Xunit;

namespace Fga.Net.Tests
{
    public class UtilityTests
    {
        [Fact]
        public void QueryBuilder_NullValues_ReturnsOriginalRequest()
        {
            var x = "/request";
            var x2 = x.BuildQueryString(("value", null), ("value1", null));

            Assert.Equal(x, x2);
        }

        [Fact]
        public void QueryBuilder_ValidValues_ReturnsCompletedString()
        {
            var x = "/request";
            var actual = x.BuildQueryString(("value", "1"), ("value2", "2"));

            var expected = x + "?value=1&value2=2";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueryBuilder_NullValue_IsFilteredOut()
        {
            var x = "/request";
            var actual = x.BuildQueryString(("value", null), ("value2", "2"));

            var expected = x + "?value2=2";

            Assert.Equal(expected, actual);
        }
    }
}
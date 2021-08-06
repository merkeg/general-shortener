using FluentAssertions;
using general_shortener.Utils;
using Xunit;

namespace general_shortener_tests.Utils
{
    public class StringUtilsTest
    {


        [Theory]
        [InlineData("https://docs.microsoft.com/de-de/dotnet/api/system.tuple?view=net-5.0", true)]
        [InlineData("https://github.com/merkeg/general-shortener", true)]
        [InlineData("https://localhost:5001/rWQBl8", true)]
        [InlineData("https://www.youtube.com/watch?v=U6p8qHs8WGA", true)]
        [InlineData("file:///jdabkjadsgb", false)]
        [InlineData("htpp//www.youtube.com", false)]
        [InlineData("www.youtube.com", false)]
        [InlineData("Wie geht es dir?", false)]
        public void TestUris(string uri, bool expectedResult)
        {
            uri.ValidateUri().Should().Be(expectedResult);
        }
        
        [Fact]
        public void TestSlugGeneration()
        {
            for (int i = 1; i < 20; i++)
            {
                StringUtils.CreateSlug(i).Length.Should().Be(i);
            }
        }
        
        
    }
}
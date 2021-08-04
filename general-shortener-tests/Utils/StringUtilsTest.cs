using System;
using System.Collections.Generic;
using FluentAssertions;
using general_shortener.Utils;
using Xunit;

namespace general_shortener_tests.Utils
{
    public class StringUtilsTest
    {
        public static readonly List<Tuple<string, bool>> Uris = new()
        {
            new("https://docs.microsoft.com/de-de/dotnet/api/system.tuple?view=net-5.0", true),
            new("https://github.com/merkeg/general-shortener", true),
            new("https://localhost:5001/rWQBl8", true),
            new("https://www.youtube.com/watch?v=U6p8qHs8WGA", true),
            new("file:///jdabkjadsgb", false),
            new("htpp//www.youtube.com", false),
            new("www.youtube.com", false),
            new("Wie geht es dir?", false),
        };

        [Fact]
        public void TestUris()
        {
            foreach (Tuple<string,bool> tuple in Uris)
            {
                tuple.Item1.ValidateUri().Should().Be(tuple.Item2);
            }
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
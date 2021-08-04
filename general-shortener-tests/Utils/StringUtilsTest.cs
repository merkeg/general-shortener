using System;
using System.Collections.Generic;
using general_shortener.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace general_shortener_tests.Utils
{
    [TestClass]
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
        
        [TestMethod]
        public void TestUris()
        {
            foreach (Tuple<string,bool> tuple in Uris)
            {
                if (tuple.Item2)
                {
                    Assert.IsTrue(tuple.Item1.ValidateUri());
                }
                else
                {
                    Assert.IsFalse(tuple.Item1.ValidateUri());
                }
            }
        }

        [TestMethod]
        public void TestSlugGeneration()
        {
            for (int i = 1; i < 20; i++)
            {
                Assert.IsTrue(StringUtils.CreateSlug(i).Length == i);
            }
        }
    }
}
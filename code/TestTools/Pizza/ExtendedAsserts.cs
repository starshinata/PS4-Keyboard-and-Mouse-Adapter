using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Pizza.TestTools
{
    public class ExtendedAsserts
    {
        public static void JsonsBeEquivalent(string actual, string expected)
        {

            JToken jsonActual = JToken.Parse(actual);
            JToken jsonExpected = JToken.Parse(expected);

            List<string> actualJsonChildren = GetChildrenKeys(jsonActual);
            List<string> expectedJsonChildren = GetChildrenKeys(jsonExpected);

            if (actualJsonChildren.Count != expectedJsonChildren.Count)
            {

                string actualIsMissing = string.Join(",", GetListDifference(expectedJsonChildren, actualJsonChildren));
                string expectedIsMissing = string.Join(",", GetListDifference(actualJsonChildren, expectedJsonChildren));


                Assert.Fail("children mismatch: actual JSON has {0} children, expected JSON has {1} children" +
                    "\n actual is missing {2}," +
                    "\n expected is missing {3},",
                    actualJsonChildren.Count,
                    expectedJsonChildren.Count,
                    actualIsMissing,
                    expectedIsMissing);
            }

            /*
             var newActual = new JArray(actual.Children().OrderBy(c => c.ToString()));
             var newExpected = new JArray(expected.Children().OrderBy(c => c.ToString()));
             AssertionExtensions.Should(newActual).BeEquivalentTo(newExpected);
            */

            AssertionExtensions.Should(jsonActual).BeEquivalentTo(jsonExpected);
        }

        private static IEnumerable<string> GetListDifference(List<string> first, List<string> second)
        {
            return first.Where(item => !second.Contains(item)).ToList();

        }

        private static List<string> GetChildrenKeys(JToken jtoken)
        {
            return ((IDictionary<string, JToken>)jtoken).Keys.ToList();
        }
    }
}

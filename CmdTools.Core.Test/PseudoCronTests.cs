using CmdTools.Core.Helpers;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CmdTools.Core.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(DivideCases))]
        public void TestSimpleExpression(string year, string month, string day, string dayOfWeek, DateTime currentDate, bool expectedResult)
        {
            Assert.That(PseudoCronEvaluetor.CheckExpresison(year, month, day, dayOfWeek, currentDate), Is.EqualTo(expectedResult));
        }

        public static object[] DivideCases =
        {
            new object[] { "*", "*", "*", "*", new DateTime(2024, 1, 1), true },
            new object[] { "2024", "1", "1", "*", new DateTime(2024, 1, 1), true },
            new object[] { "2025", "*", "5", "*", new DateTime(2025, 5, 5), true },
            new object[] { "2025", "*", "5", "*", new DateTime(2025, 5, 4), false },
            new object[] { "*", "2", "L", "*", new DateTime(2024, 2, 29), true },
            new object[] { "*", "2", "L", "*", new DateTime(2025, 2, 28), true },
            new object[] { "*", "2", "L", "*", new DateTime(2024, 2, 28), false },
            new object[] { "*", "2", "30", "*", new DateTime(2024, 2, 29), false },
            new object[] { "2024", "4", "9", "*", new DateTime(2024, 4, 9), true },
        };
    }
}
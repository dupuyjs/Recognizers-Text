﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DateObject = System.DateTime;

namespace Microsoft.Recognizers.Text.DateTime.English.Tests
{
    [TestClass]
    public class TestMergedParser
    {
        private readonly IExtractor extractor = new BaseMergedExtractor(new EnglishMergedExtractorConfiguration(), DateTimeOptions.None);
        private readonly IDateTimeParser parser = new BaseMergedParser(new EnglishMergedParserConfiguration());

        readonly DateObject refrenceDate;

        public TestMergedParser()
        {
            refrenceDate = new DateObject(2016, 11, 7);
        }

        public void BasicTest(string text, string type)
        {
            var er = extractor.Extract(text);
            Assert.AreEqual(1, er.Count);
            var pr = parser.Parse(er[0], refrenceDate);
            Assert.AreEqual(type, pr.Type.Replace("datetimeV2.",""));
        }

        public void BasicTestWithTwoResults(string text, string type1, string type2)
        {
            var er = extractor.Extract(text);
            Assert.AreEqual(2, er.Count);
            var pr = parser.Parse(er[0], refrenceDate);
            Assert.AreEqual(type1, pr.Type.Replace("datetimeV2.", ""));
            pr = parser.Parse(er[1], refrenceDate);
            Assert.AreEqual(type2, pr.Type.Replace("datetimeV2.", ""));
        }

        [TestMethod]
        public void TestMergedParse()
        {
            BasicTest("on Friday in the afternoon", Constants.SYS_DATETIME_DATETIMEPERIOD);
            BasicTest("on Friday for 3 in the afternoon", Constants.SYS_DATETIME_DATETIME);
        }

        [TestMethod]
        public void TestMergedParseIn()
        {
            BasicTest("schedule a meeting in 8 minutes", Constants.SYS_DATETIME_DATETIME);
            BasicTest("schedule a meeting in 10 hours", Constants.SYS_DATETIME_DATETIME);
            BasicTest("schedule a meeting in 10 days", Constants.SYS_DATETIME_DATE);
            BasicTest("schedule a meeting in 3 weeks", Constants.SYS_DATETIME_DATEPERIOD);
            BasicTest("schedule a meeting in 3 months", Constants.SYS_DATETIME_DATEPERIOD);
            BasicTest("I'll be out in 3 year", Constants.SYS_DATETIME_DATEPERIOD);
        }

        [TestMethod]
        public void TestMergedParseAfterBefore()
        {
            BasicTest("after 8pm", Constants.SYS_DATETIME_TIMEPERIOD);
            BasicTest("before 8pm", Constants.SYS_DATETIME_TIMEPERIOD);
            BasicTest("since 8pm", Constants.SYS_DATETIME_TIMEPERIOD);
        }

        [TestMethod]
        public void TestMergedParseInvalidDatetime()
        {
            BasicTest("2016-2-30", Constants.SYS_DATETIME_DATE);
            //only 2015-1 is extracted
            BasicTest("2015-1-32", Constants.SYS_DATETIME_DATEPERIOD);
            //only 2017 is extracted
            BasicTest("2017-13-12", Constants.SYS_DATETIME_DATEPERIOD);
        }

        [TestMethod]
        public void TestMergedParseWithTwoResults()
        {
            BasicTestWithTwoResults("Change July 22nd meeting in Bellevue to August 22nd", Constants.SYS_DATETIME_DATE,
                Constants.SYS_DATETIME_DATE);
            BasicTestWithTwoResults("on Friday for 3 in Bellevue in the afternoon", Constants.SYS_DATETIME_DATE,
                Constants.SYS_DATETIME_TIMEPERIOD);
        }
    }
}

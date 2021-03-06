﻿using System;
using DateObject = System.DateTime;

namespace Microsoft.Recognizers.Text.DateTime.Spanish
{
    public class DateTimePeriodParser : BaseDateTimePeriodParser
    {
        public DateTimePeriodParser(IDateTimePeriodParserConfiguration configuration) : base(configuration)
        {
        }

        protected override DateTimeResolutionResult ParseSpecificTimeOfDay(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            var trimedText = text.Trim().ToLowerInvariant();

            // handle morning, afternoon..
            int beginHour, endHour, endMin = 0;
            string timeStr;
            if (!this.Config.GetMatchedTimeRange(trimedText, out timeStr, out beginHour, out endHour, out endMin))
            {
                return ret;
            }

            var match = this.Config.SpecificTimeOfDayRegex.Match(trimedText);
            if (match.Success && match.Index == 0 && match.Length == trimedText.Length)
            {
                var swift = this.Config.GetSwiftPrefix(trimedText);

                var date = referenceTime.AddDays(swift).Date;
                int day = date.Day, month = date.Month, year = date.Year;

                ret.Timex = FormatUtil.FormatDate(date) + timeStr;
                ret.FutureValue =
                    ret.PastValue =
                        new Tuple<DateObject, DateObject>(DateObject.MinValue.SafeCreateFromValue(year, month, day, beginHour, 0, 0),
                            DateObject.MinValue.SafeCreateFromValue(year, month, day, endHour, endMin, endMin));
                ret.Success = true;
                return ret;
            }

            var startIndex = trimedText.IndexOf("mañana", StringComparison.Ordinal) == 0 ? 6 : 0;

            // handle Date followed by morning, afternoon
            // Add handling code to handle morning, afternoon followed by Date
            // Add handling code to handle early/late morning, afternoon
            match = this.Config.TimeOfDayRegex.Match(trimedText.Substring(startIndex));
            if (match.Success)
            {
                var beforeStr = trimedText.Substring(0, match.Index + startIndex).Trim();
                var ers = this.Config.DateExtractor.Extract(beforeStr);
                if (ers.Count == 0)
                {
                    return ret;
                }

                var pr = this.Config.DateParser.Parse(ers[0], referenceTime);
                var futureDate = (DateObject)((DateTimeResolutionResult)pr.Value).FutureValue;
                var pastDate = (DateObject)((DateTimeResolutionResult)pr.Value).PastValue;

                ret.Timex = pr.TimexStr + timeStr;

                ret.FutureValue =
                    new Tuple<DateObject, DateObject>(
                        DateObject.MinValue.SafeCreateFromValue(futureDate.Year, futureDate.Month, futureDate.Day, beginHour, 0, 0),
                        DateObject.MinValue.SafeCreateFromValue(futureDate.Year, futureDate.Month, futureDate.Day, endHour, endMin, endMin));

                ret.PastValue =
                    new Tuple<DateObject, DateObject>(
                        DateObject.MinValue.SafeCreateFromValue(pastDate.Year, pastDate.Month, pastDate.Day, beginHour, 0, 0),
                        DateObject.MinValue.SafeCreateFromValue(pastDate.Year, pastDate.Month, pastDate.Day, endHour, endMin, endMin));

                ret.Success = true;

                return ret;
            }

            return ret;
        }
    }
}

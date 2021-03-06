﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text.Number.English;
using Microsoft.Recognizers.Definitions.English;

namespace Microsoft.Recognizers.Text.DateTime.English
{
    public class EnglishDateTimePeriodExtractorConfiguration : IDateTimePeriodExtractorConfiguration
    {
        public EnglishDateTimePeriodExtractorConfiguration()
        {
            CardinalExtractor = Number.English.CardinalExtractor.GetInstance();
            SingleDateExtractor = new BaseDateExtractor(new EnglishDateExtractorConfiguration());
            SingleTimeExtractor = new BaseTimeExtractor(new EnglishTimeExtractorConfiguration());
            SingleDateTimeExtractor = new BaseDateTimeExtractor(new EnglishDateTimeExtractorConfiguration());
            DurationExtractor=new BaseDurationExtractor(new EnglishDurationExtractorConfiguration());
        }
        
        private static readonly Regex[] SimpleCases = 
        {
            EnglishTimePeriodExtractorConfiguration.PureNumFromTo,
            EnglishTimePeriodExtractorConfiguration.PureNumBetweenAnd
        };

        public IEnumerable<Regex> SimpleCasesRegex => SimpleCases;

        public Regex PrepositionRegex => EnglishTimePeriodExtractorConfiguration.PrepositionRegex;

        public Regex TillRegex => EnglishTimePeriodExtractorConfiguration.TillRegex;

        private static readonly Regex PeriodTimeOfDayRegex = 
            new Regex(DateTimeDefinitions.PeriodTimeOfDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex PeriodSpecificTimeOfDayRegex = 
            new Regex(DateTimeDefinitions.PeriodSpecificTimeOfDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public Regex TimeOfDayRegex => PeriodTimeOfDayRegex;

        public Regex SpecificTimeOfDayRegex => PeriodSpecificTimeOfDayRegex;

        private static readonly Regex TimeTimeUnitRegex =
            new Regex(DateTimeDefinitions.TimeUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex TimeFollowedUnit = 
            new Regex(DateTimeDefinitions.TimeFollowedUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex TimeNumberCombinedWithUnit = 
            new Regex(DateTimeDefinitions.TimeNumberCombinedWithUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex PeriodTimeOfDayWithDateRegex = 
            new Regex(DateTimeDefinitions.PeriodTimeOfDayWithDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex RelativeTimeUnitRegex = 
            new Regex(DateTimeDefinitions.RelativeTimeUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public Regex FollowedUnit => TimeFollowedUnit;

        Regex IDateTimePeriodExtractorConfiguration.NumberCombinedWithUnit => TimeNumberCombinedWithUnit;
        
        Regex IDateTimePeriodExtractorConfiguration.TimeUnitRegex => TimeTimeUnitRegex;

        Regex IDateTimePeriodExtractorConfiguration.RelativeTimeUnitRegex => RelativeTimeUnitRegex;

        public Regex PastPrefixRegex => EnglishDatePeriodExtractorConfiguration.PastPrefixRegex;

        public Regex NextPrefixRegex => EnglishDatePeriodExtractorConfiguration.NextPrefixRegex;

        public Regex WeekDayRegex => new Regex(DateTimeDefinitions.WeekDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Regex IDateTimePeriodExtractorConfiguration.PeriodTimeOfDayWithDateRegex => PeriodTimeOfDayWithDateRegex;

        public IExtractor CardinalExtractor { get; }

        public IExtractor SingleDateExtractor { get; }

        public IExtractor SingleTimeExtractor { get; }

        public IExtractor SingleDateTimeExtractor { get; }

        public IExtractor DurationExtractor { get; }

        //TODO: these three methods are the same in DatePeriod, should be abstracted
        public bool GetFromTokenIndex(string text, out int index)
        {
            index = -1;
            if (text.EndsWith("from"))
            {
                index = text.LastIndexOf("from", StringComparison.Ordinal);
                return true;
            }
            return false;
        }

        public bool GetBetweenTokenIndex(string text, out int index)
        {
            index = -1;
            if (text.EndsWith("between"))
            {
                index = text.LastIndexOf("between", StringComparison.Ordinal);
                return true;
            }
            return false;
        }

        public bool HasConnectorToken(string text)
        {
            var match = Regex.Match(text, DateTimeDefinitions.RangeConnectorRegex);
            return match.Success && match.Length == text.Trim().Length;
        }
    }
}

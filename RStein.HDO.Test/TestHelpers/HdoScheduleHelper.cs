using System;
using System.Collections.Generic;

namespace RStein.HDO.Test.TestHelpers
{
  public static class HdoScheduleHelper
  {
    public const string TEST_HDO_PROVIDER_NAME = "Test HDO provider";

    public static HdoSchedule CreateDefaultNonEmptyHdoSchedule(Func<DateTime> getTimeFunc = null)
    {
      
      return new HdoSchedule(TEST_HDO_PROVIDER_NAME, new[]
                             {
                               new HdoScheduleIntervalDefinition(new List<HdoScheduleIntervalItem>
                               {
                                 new HdoScheduleIntervalItem(0, 0, 7, 55, getTimeFunc),
                                 new HdoScheduleIntervalItem(8, 50, 11, 55, getTimeFunc),
                                 new HdoScheduleIntervalItem(12, 50, 14, 35, getTimeFunc),
                                 new HdoScheduleIntervalItem(15, 35, 19, 50, getTimeFunc),
                                 new HdoScheduleIntervalItem(20, 50, 23, 59, getTimeFunc)
                               }, new[] {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday}),

                               new HdoScheduleIntervalDefinition(new List<HdoScheduleIntervalItem>
                               {
                                 new HdoScheduleIntervalItem(0, 0, 7, 55, getTimeFunc),
                                 new HdoScheduleIntervalItem(8, 50, 10, 55, getTimeFunc),
                                 new HdoScheduleIntervalItem(11, 50, 14, 10, getTimeFunc),
                                 new HdoScheduleIntervalItem(15, 10, 19, 15, getTimeFunc),
                                 new HdoScheduleIntervalItem(20, 15, 23, 59, getTimeFunc)
                               },
                               new[] {DayOfWeek.Saturday, DayOfWeek.Sunday},
                               getTimeFunc)

                             },
                             getTimeFunc);
    }
  }
}
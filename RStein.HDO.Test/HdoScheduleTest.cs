using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static RStein.HDO.Test.TestHelpers.HdoScheduleHelper;

namespace RStein.HDO.Test
{
  [TestFixture]
  public class HdoScheduleTest
  {
    [TestCase(null)]
    [TestCase("")]
    public void Ctor_When_Provider_Name_Is_Invalid_Then_Throws_ArgumentException(string invalidProviderName)
    {
      Assert.Catch<ArgumentException>(() =>
      {
        var _ = new HdoSchedule(invalidProviderName, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      });
    }

    [Test]
    public void Ctor_When_ScheduleIntervalCollection_Is_Null_Then_Throws_ArgumentNullException()
    {
      IEnumerable<HdoScheduleIntervalDefinition> nullScheduleIntervals = null;
      
      Assert.Catch<ArgumentNullException>(() =>
      {
        var _ = new HdoSchedule(TEST_HDO_PROVIDER_NAME, nullScheduleIntervals);
      });
    }
    
    [Test]
    public void ToString_When_Called_Then_Result_Is_Not_Null_Or_Empty()
    {
      var schedule = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());

      var scheduleString = schedule.ToString();

      Assert.That(scheduleString, Is.Not.Null.Or.Empty);
    }
    
    [Test]
    public void Equals_When_Same_Instances_Then_Returns_True()
    {
      var schedule1 = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      var schedule2 = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      
      bool areEqual = schedule1.Equals(schedule2);
      
      Assert.IsTrue(areEqual);
    }

    [Test]
    public void Equals_When_Different_Instance_Then_Returns_False()
    {
      var schedule1 = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      var schedule2 = new HdoSchedule(TEST_HDO_PROVIDER_NAME + "diff", Enumerable.Empty<HdoScheduleIntervalDefinition>());
      
      bool areEqual = schedule1.Equals(schedule2);
      
      Assert.IsFalse(areEqual);
    }

    [Test]
    public void IsHdoActive_When_Empty_Schedule_Then_Returns_False()
    {
      var schedule = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());

      var isHdoActive = schedule.IsHdoActive();

      Assert.IsFalse(isHdoActive);
    }

    [Test]
    public void IsHdoTime_When_Empty_Schedule_Then_Returns_False()
    {
      var toCheckDate = new DateTime(2020, 12, 12, 10, 20, 00);
      var schedule = new HdoSchedule(TEST_HDO_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());

      var isHdoTime = schedule.IsHdoTime(toCheckDate);

      Assert.IsFalse(isHdoTime);
    }

    
    [TestCase(0, 1)]
    [TestCase(7, 54)]
    [TestCase(7, 55)]
    [TestCase(9, 0)]
    [TestCase(9, 1)]
    [TestCase(9, 45)]
    [TestCase(13, 30)]
    public void IsHdoActive_When_Schedule_Has_Active_Hdo_In_Time_Slot_Then_Returns_True(int hour, int minute)
    {
      var toCheckDate = new DateTime(2020, 12, 12, hour, minute, 00);
      var schedule = CreateDefaultNonEmptyHdoSchedule(() => toCheckDate);

      var isHdoActive = schedule.IsHdoActive();

      Assert.IsTrue(isHdoActive);
    }

    [TestCase(7, 56)]
    [TestCase(8, 0)]
    [TestCase(8, 1)]
    [TestCase(20, 0)]
    [TestCase(20, 1)]
    public void IsHdoActive_When_Schedule_Does_Not_Have_Active_Hdo_In_Time_Slot_Then_Returns_False(int hour, int minute)
    {
      var toCheckDate = new DateTime(2020, 12, 10, hour, minute, 00);
      var schedule = CreateDefaultNonEmptyHdoSchedule(() => toCheckDate);

      var isHdoActive = schedule.IsHdoActive();

      Assert.IsFalse(isHdoActive);
    }
  }
}
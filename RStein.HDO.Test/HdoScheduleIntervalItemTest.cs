using System;
using NUnit.Framework;

namespace RStein.HDO.Test
{
  [TestFixture]
  public class HdoScheduleIntervalItemTest
  {
    [TestCase(-1)]
    [TestCase(-100)]
    [TestCase(25)]
    [TestCase(30)]
    public void When_Invalid_BeginHour_Then_Throws_Argument_Exception(int invalidBeginHour)
    {
      var beginMinute = 1;
      var endHour = 1;
      var endMinute = 1;

      Assert.Catch<ArgumentOutOfRangeException>(() =>
      {
        var _ = new HdoScheduleIntervalItem(invalidBeginHour, beginMinute, endHour, endMinute);
      });
    }

    [TestCase(-1)]
    [TestCase(-100)]
    [TestCase(61)]
    [TestCase(62)]
    [TestCase(100)]
    public void When_Invalid_BeginMinute_Then_Throws_Argument_Exception(int invalidBeginMinute)
    {
      var beginHour = 1;
      var endHour = 1;
      var endMinute = 1;

      Assert.Catch<ArgumentOutOfRangeException>(() =>
      {
        var _ = new HdoScheduleIntervalItem(beginHour, invalidBeginMinute, endHour, endMinute);
      });
    }

    [TestCase(-1)]
    [TestCase(-100)]
    [TestCase(25)]
    [TestCase(30)]
    public void When_Invalid_EndHour_Then_Throws_Argument_Exception(int invalidEndHour)
    {
      var beginHour = 1;
      var beginMinute = 1;
      var endMinute = 1;

      Assert.Catch<ArgumentOutOfRangeException>(() =>
      {
        var _ = new HdoScheduleIntervalItem(beginHour, beginMinute, invalidEndHour, endMinute);
      });
    }

    [TestCase(-1)]
    [TestCase(-100)]
    [TestCase(61)]
    [TestCase(62)]
    [TestCase(100)]
    public void When_Invalid_EndMinute_Then_Throws_Argument_Exception(int invalidEndMinute)
    {
      var beginHour = 1;
      var beginMinute = 1;
      var endHour = 1;

      Assert.Catch<ArgumentOutOfRangeException>(() =>
      {
        var _ = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, invalidEndMinute);
      });
    }

    [TestCase(1, 2, 1, 1)]
    [TestCase(10, 30, 10, 0)]
    [TestCase(10, 30, 9, 0)]
    [TestCase(10, 30, 9, 59)]
    [TestCase(10, 30, 10, 29)]
    [TestCase(10, 30, 0, 0)]
    public void When_End_Interval_Is_Before_Begin_Interval_Then_Throws_ArgumentException(int beginHour,
      int beginMinute,
      int endHour,
      int endMinute)
    {
      Assert.Catch<ArgumentException>(() =>
      {
        var _ = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);
      });
    }

    [Test]
    public void BeginHour_When_Get_Then_Returns_Ctor_Argument()
    {
      var expectedBeginHour = 1;
      var beginMinute = 2;
      var endHour = 3;
      var endMinute = 4;
      var interval = new HdoScheduleIntervalItem(expectedBeginHour, beginMinute, endHour, endMinute);

      var intervalBeginHour = interval.BeginHour;

      Assert.AreEqual(expectedBeginHour, intervalBeginHour);
    }

    [Test]
    public void BeginMinute_When_Get_Then_Returns_Ctor_Argument()
    {
      var beginHour = 1;
      var expectedBeginMinute = 2;
      var endHour = 3;
      var endMinute = 4;

      var interval = new HdoScheduleIntervalItem(beginHour, expectedBeginMinute, endHour, endMinute);

      var intervalBeginMinute = interval.BeginMinute;
      Assert.AreEqual(expectedBeginMinute, intervalBeginMinute);
    }

    [Test]
    public void EndHour_When_Get_Then_Returns_Ctor_Argument()
    {
      var beginHour = 1;
      var beginMinute = 2;
      var expectedEndHour = 3;
      var endMinute = 4;
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, expectedEndHour, endMinute);

      var intervalEndHour = interval.EndHour;

      Assert.AreEqual(expectedEndHour, intervalEndHour);
    }

    [Test]
    public void EndMinute_When_Get_Then_Returns_Ctor_Argument()
    {
      var beginHour = 1;
      var beginMinute = 2;
      var endHour = 3;
      var expectedEndMinute = 4;
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, expectedEndMinute);

      var intervalEndMinute = interval.EndMinute;

      Assert.AreEqual(expectedEndMinute, intervalEndMinute);
    }


    [Test]
    public void Equals_When_Instances_Are_Equal_Then_Returns_True()
    {
      var beginHour = 1;
      var beginMinute = 2;
      var endHour = 3;
      var endMinute = 4;

      var interval1 = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);
      var interval2 = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);

      var areEqual = interval1.Equals(interval2);

      Assert.IsTrue(areEqual);
    }

    [Test]
    public void Equals_When_Instances_Are_Not_Equal_Then_Returns_False()
    {
      var beginHour = 1;
      var beginMinute = 2;
      var endHour = 3;
      var endMinute = 4;
      var endMinuteDiffInstance2 = endMinute + 1;

      var interval1 = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);
      var interval2 = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinuteDiffInstance2);

      var areEqual = interval1.Equals(interval2);

      Assert.IsFalse(areEqual);
    }

    [TestCase(8, 1)]
    [TestCase(8, 2)]
    [TestCase(8, 30)]
    [TestCase(8, 59)]
    [TestCase(9, 0)]
    [TestCase(9, 1)]
    [TestCase(10, 2)]
    public void IsHdoTime_When_Hdo_Time_Then_Returns_True(int hour,
                                                          int minute)
    {
      var beginHour = 8;
      var beginMinute = 1;
      var endHour = 10;
      var endMinute = 2;
      var timeToCheck = new DateTime(2020, 10, 1, hour, minute, 0);
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);

      var isHdoTime = interval.IsHdoTime(timeToCheck);

      Assert.IsTrue(isHdoTime);
    }

    [TestCase(8, 0)]
    [TestCase(7, 59)]
    [TestCase(0, 0)]
    [TestCase(10, 3)]
    [TestCase(11, 10)]
    [TestCase(14, 34)]
    [TestCase(23, 59)]
    public void IsHdoTime_When_Not_Hdo_Time_Then_Returns_False(int hour,
                                                               int minute)
    {
      var beginHour = 8;
      var beginMinute = 1;
      var endHour = 10;
      var endMinute = 2;
      var timeToCheck = new DateTime(2020, 10, 1, hour, minute, 0);
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);

      var isHdoTime = interval.IsHdoTime(timeToCheck);

      Assert.IsFalse(isHdoTime);
    }

    [TestCase(8, 1)]
    [TestCase(8, 2)]
    [TestCase(8, 30)]
    [TestCase(8, 59)]
    [TestCase(9, 0)]
    [TestCase(9, 1)]
    [TestCase(10, 2)]
    public void IsHdoActive_When_Hdo_Time_Then_Returns_True(int hour,
                                                            int minute)
    {
      var beginHour = 8;
      var beginMinute = 1;
      var endHour = 10;
      var endMinute = 2;
      var timeToCheck = new DateTime(2020, 10, 1, hour, minute, 0);
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute, () => timeToCheck);

      var isHdoActive = interval.IsHdoActive();

      Assert.IsTrue(isHdoActive);
    }


    [TestCase(8, 0)]
    [TestCase(7, 59)]
    [TestCase(0, 0)]
    [TestCase(10, 3)]
    [TestCase(11, 10)]
    [TestCase(14, 34)]
    [TestCase(23, 59)]
    public void IsHdoActive_When_Not_Hdo_Time_Then_Returns_False(int hour,
                                                                 int minute)
    {
      var beginHour = 8;
      var beginMinute = 1;
      var endHour = 10;
      var endMinute = 2;
      var timeToCheck = new DateTime(2020, 10, 1, hour, minute, 0);
      var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute, () => timeToCheck);

      var isHdoActive = interval.IsHdoActive();

      Assert.IsFalse(isHdoActive);
    }

    [Test]
    public void ToString_When_Called_Then_Result_Is_Not_Null_Or_Empty()
    {
      {
        var beginHour = 8;
        var beginMinute = 1;
        var endHour = 10;
        var endMinute = 2;
        var interval = new HdoScheduleIntervalItem(beginHour, beginMinute, endHour, endMinute);

        var intervalString = interval.ToString();

        Assert.That(intervalString, Is.Not.Null.Or.Empty);
      }
    }
  }
}
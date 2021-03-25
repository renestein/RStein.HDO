using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RStein.HDO.Infrastructure;

namespace RStein.HDO
{
  public class HdoScheduleIntervalDefinition : IEquatable<HdoScheduleIntervalDefinition>
  {
    private const string DAY_SEPARATOR = ", ";
    private const char START_DAYS_CHAR = '[';
    private const char END_DAYS_CHAR = ']';
    private IDictionary<string, object> _additionalValues;
    private readonly Func<DateTime> _getTimeFunc;

    public HdoScheduleIntervalDefinition(IEnumerable<HdoScheduleIntervalItem> scheduleIntervalItems,
                                         IEnumerable<DayOfWeek> applyToDayOfWeeks,
                                         Func<DateTime> getTimeFunc = null,
                                         IDictionary<string, object> additionalValues = null)
    {
      ScheduleIntervalItems = scheduleIntervalItems ?? throw new ArgumentNullException(nameof(scheduleIntervalItems));
      ApplyToDayOfWeeks = applyToDayOfWeeks ?? throw new ArgumentNullException(nameof(applyToDayOfWeeks));
      _getTimeFunc = getTimeFunc ?? DateTimeUtils.DefaultGetTimeFunc;
      _additionalValues = additionalValues;
    }

    public IEnumerable<HdoScheduleIntervalItem> ScheduleIntervalItems
    {
      get;
    }

    public IEnumerable<DayOfWeek> ApplyToDayOfWeeks
    {
      get;
    }

    public IDictionary<string, object> AdditionalValues =>
      _additionalValues = _additionalValues ?? new Dictionary<string, object>();

    public virtual bool IsHdoTime(DateTime timeToCheck)
    {
      return ApplyToDayOfWeeks.Contains(timeToCheck.DayOfWeek) &&
             ScheduleIntervalItems.Any(definition => definition.IsHdoTime(timeToCheck));
    }

    public virtual bool IsHdoActive() => IsHdoTime(_getTimeFunc());

    public bool Equals(HdoScheduleIntervalDefinition other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return ApplyToDayOfWeeks.SequenceEqual(other.ApplyToDayOfWeeks) &&
             ScheduleIntervalItems.SequenceEqual(other.ScheduleIntervalItems);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      if (obj.GetType() != this.GetType())
      {
        return false;
      }

      return Equals((HdoScheduleIntervalDefinition) obj);
    }

    public override int GetHashCode()
    {
      var hashCode = ApplyToDayOfWeeks.Aggregate(0, (i,
                                                     day) => (i * 397) ^ (int) day);
      return ScheduleIntervalItems.Aggregate(hashCode, (i,
                                                        item) => (i * 397) ^ item.GetHashCode());
    }

    public static bool operator ==(HdoScheduleIntervalDefinition left,
                                   HdoScheduleIntervalDefinition right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(HdoScheduleIntervalDefinition left,
                                   HdoScheduleIntervalDefinition right)
    {
      return !Equals(left, right);
    }

    public override string ToString()
    {
      var sb = new StringBuilder().Append(START_DAYS_CHAR);
      
      var sbWithDays = ApplyToDayOfWeeks.Aggregate(sb, (builder, day) =>
                                    {
                                      builder.Append(day.ToString());
                                      builder.Append(DAY_SEPARATOR);
                                      return builder;
                                    }, builder =>
                                    {
                                      builder.Length = builder.Length - DAY_SEPARATOR.Length;
                                      builder.Append(END_DAYS_CHAR);
                                      builder.AppendLine();
                                      return builder;
                                    });

      return ScheduleIntervalItems.Aggregate(sbWithDays, (builder, item) => sbWithDays.AppendLine(item.ToString()),
                                             builder => builder.ToString());
    }
  }
}
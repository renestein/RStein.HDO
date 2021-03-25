using System;
using System.Collections.Generic;
using RStein.HDO.Infrastructure;

namespace RStein.HDO
{
  public class HdoScheduleIntervalItem : IEquatable<HdoScheduleIntervalItem>
  {
    private const int MIN_ALLOWED_HOUR = 0;
    private const int MAX_ALLOWED_HOUR = 24;

    private const int MIN_ALLOWED_MINUTE = 0;
    private const int MAX_ALLOWED_MINUTE = 60;

    private readonly Func<DateTime> _getDateTimeFunc;
    private IDictionary<string, object> _additionalValues;

    public HdoScheduleIntervalItem(int beginHour,
                                   int beginMinute,
                                   int endHour,
                                   int endMinute,
                                   Func<DateTime> getDateTimeFunc = null,
                                   IDictionary<string, object> additionalValues = null)
    {
      if (beginHour < MIN_ALLOWED_HOUR || beginHour > MAX_ALLOWED_HOUR)
      {
        throw new ArgumentOutOfRangeException(nameof(beginHour));
      }

      if (beginMinute < MIN_ALLOWED_MINUTE || beginMinute > MAX_ALLOWED_MINUTE)
      {
        throw new ArgumentOutOfRangeException(nameof(beginMinute));
      }
      
      if (endHour < MIN_ALLOWED_HOUR || endHour > MAX_ALLOWED_HOUR)
      {
        throw new ArgumentOutOfRangeException(nameof(endHour));
      }

      if (endMinute < MIN_ALLOWED_MINUTE || endMinute > MAX_ALLOWED_MINUTE)
      {
        throw new ArgumentOutOfRangeException(nameof(endMinute));
      }

      if (endHour < beginHour || ((endHour == beginHour) && (endMinute < beginMinute)))
      {
        throw new ArgumentException($"End interval '{endHour}:{endMinute}' should represent time after '{beginHour}:{beginMinute}' begin interval");
      }
      BeginHour = beginHour;
      EndHour = endHour;
      BeginMinute = beginMinute;
      EndMinute = endMinute;
      _getDateTimeFunc = getDateTimeFunc ?? DateTimeUtils.DefaultGetTimeFunc;
      _additionalValues = additionalValues;
    }

    public int BeginHour
    {
      get;
    }

    public int EndHour
    {
      get;
    }

    public int BeginMinute
    {
      get;
    }

    public int EndMinute
    {
      get;
    }

    public virtual bool IsHdoTime(DateTime timeToCheck)
    {
      return ((timeToCheck.Hour == BeginHour && timeToCheck.Minute >= BeginMinute) || (timeToCheck.Hour > BeginHour)
            && ((timeToCheck.Hour == EndHour && timeToCheck.Minute <= EndMinute) || (timeToCheck.Hour < EndHour)));

    }

    public virtual bool IsHdoActive() => IsHdoTime(_getDateTimeFunc());
    public IDictionary<string, object> AdditionalValues => _additionalValues = _additionalValues ?? new Dictionary<string, object>();

    public override string ToString()
    {
      return $"{BeginHour:00}:{BeginMinute:00} - {EndHour:00}:{EndMinute:00}";
    }

    public bool Equals(HdoScheduleIntervalItem other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return BeginHour == other.BeginHour && EndHour == other.EndHour && BeginMinute == other.BeginMinute && EndMinute == other.EndMinute;
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

      if (obj.GetType() != GetType())
      {
        return false;
      }

      return Equals((HdoScheduleIntervalItem) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = BeginHour;
        hashCode = (hashCode * 397) ^ EndHour;
        hashCode = (hashCode * 397) ^ BeginMinute;
        hashCode = (hashCode * 397) ^ EndMinute;
        return hashCode;
      }
    }

    public static bool operator ==(HdoScheduleIntervalItem left,
                                   HdoScheduleIntervalItem right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(HdoScheduleIntervalItem left,
                                   HdoScheduleIntervalItem right)
    {
      return !Equals(left, right);
    }
  }
}
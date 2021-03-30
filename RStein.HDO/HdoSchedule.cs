using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RStein.HDO.Infrastructure;

namespace RStein.HDO
{
  public class HdoSchedule : IEquatable<HdoSchedule>
  {
    private const string SCHEDULE_FORMAT = "---- HDO schedule - {0} ---- ";
    private readonly Func<DateTime> _getTimeFunc;
    private IDictionary<string, object> _additionalValues;

    public HdoSchedule(string providerName,
                       IEnumerable<HdoScheduleIntervalDefinition> scheduleIntervalDefinitions,
                       Func<DateTime> getTimeFunc = null,
                       IDictionary<string, object> additionalValues = null)
    {
      if (string.IsNullOrWhiteSpace(providerName))
      {
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(providerName));
      }

      ProviderName = providerName;
      ScheduleIntervalDefinitions = scheduleIntervalDefinitions ?? throw new ArgumentNullException(nameof(scheduleIntervalDefinitions));
      _getTimeFunc = getTimeFunc ?? DateTimeUtils.DefaultGetTimeFunc;
      _additionalValues = additionalValues;
    }

    public string ProviderName
    {
      get;
    }

    public IEnumerable<HdoScheduleIntervalDefinition> ScheduleIntervalDefinitions
    {
      get;
    }

    public IDictionary<string, object> AdditionalValues => _additionalValues = _additionalValues ?? new Dictionary<string, object>();

    public bool IsEmpty
    {
      get
      {
        return !ScheduleIntervalDefinitions.Any() || ScheduleIntervalDefinitions.All(definition => definition.IsEmpty);
      }
    }

    public virtual bool IsHdoTime(DateTime timeToCheck) => ScheduleIntervalDefinitions.Any(definition => definition.IsHdoTime(timeToCheck));

    public virtual bool IsHdoActive() => IsHdoTime(_getTimeFunc());
    
    public bool Equals(HdoSchedule other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return ProviderName.Equals(other.ProviderName) &&
             ScheduleIntervalDefinitions.SequenceEqual(other.ScheduleIntervalDefinitions);
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

      return Equals((HdoSchedule) obj);
    }

    public override int GetHashCode()
    {
      var hash = ProviderName.GetHashCode();
      return ScheduleIntervalDefinitions.Aggregate(hash, (currentHash, definition) => (hash * 397) ^ definition.GetHashCode());
    }

    public static bool operator ==(HdoSchedule left,
                                   HdoSchedule right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(HdoSchedule left,
                                   HdoSchedule right)
    {
      return !Equals(left, right);
    }

    public override string ToString()
    {
      var sb = new StringBuilder().AppendFormat(SCHEDULE_FORMAT, ProviderName)
                                              .AppendLine();
      return ScheduleIntervalDefinitions.Aggregate(sb, (scheduleString, definition) => scheduleString.AppendLine(definition.ToString()), builder => builder.ToString());
    }
  }
}
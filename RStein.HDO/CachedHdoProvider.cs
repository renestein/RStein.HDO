using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RStein.HDO.Infrastructure;

namespace RStein.HDO
{
  public class CachedHdoProvider : IHdoScheduleProvider, IDisposable
  {
    private IHdoScheduleProvider _innerProvider;
    private readonly TimeSpan _validFor;
    private readonly ConcurrentDictionary<int, (DateTime Date, Task<HdoSchedule> Task)> _cachedTasks;
    private readonly Func<DateTime> _getTimeFunc;

    public CachedHdoProvider(IHdoScheduleProvider innerProvider, TimeSpan validFor, Func<DateTime> getTimeFunc = null)
    {
      _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
      _validFor = validFor;
      _getTimeFunc = getTimeFunc ?? DateTimeUtils.DefaultGetTimeFunc;
      _cachedTasks = new ConcurrentDictionary<int, (DateTime date, Task<HdoSchedule> task)>();
    }

    public string Name => _innerProvider.Name;

    public async Task<HdoSchedule> GetScheduleAsync(IDictionary<string, string> data)
    {
      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      var dataHash = calculateHash(data);
      if (_cachedTasks.TryGetValue(dataHash, out var currentCachedTask))
      {
        if (_getTimeFunc() < currentCachedTask.Date)
        {
          return await currentCachedTask.Task.ConfigureAwait(false);
        }

      }

      var getScheduleTask = _innerProvider.GetScheduleAsync(data);
      var schedule = await getScheduleTask.ConfigureAwait(false);
      var cachedTaskValidTo = _getTimeFunc() + _validFor;
      var cachedTask =  (cachedTaskValidTo, getScheduleTask);
      _cachedTasks.AddOrUpdate(dataHash, cachedTask, (_, __) => cachedTask);

      return schedule;
    }

    private int calculateHash(IDictionary<string, string> data)
    {
      
      unchecked
      {
        return data.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                   .Select(pair => pair.Key.GetHashCode() + pair.Value.GetHashCode())
                   .Aggregate(0, (sum, i) => sum + i); //Sum throws OverflowException
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        (_innerProvider as IDisposable)?.Dispose();
        _innerProvider = null;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
  }
}
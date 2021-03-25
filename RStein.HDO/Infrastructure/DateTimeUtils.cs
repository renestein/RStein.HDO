using System;

namespace RStein.HDO.Infrastructure
{
  public static class DateTimeUtils
  {
    public static Func<DateTime> DefaultGetTimeFunc
    {
      get;
    } = () => DateTime.Now;
  }
}
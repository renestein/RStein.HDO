using System.Collections.Generic;
using System.Threading.Tasks;

namespace RStein.HDO
{
  public interface IHdoScheduleProvider
  {
    string Name
    {
      get;
    }
    
    Task<HdoSchedule> GetScheduleAsync(IDictionary<string, string> data);
  }
}
#pragma warning disable ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RStein.HDO.CEZ;

namespace RStein.HDO.Cui
{
  class Program
  {
    static async Task Main(string[] args)
    {
      //await getCezSchedule();
      //await runCachedHdoSchedule();
      //await checkRawJson();

      //Create CEZ HDO provider
      using var provider = new CezHdoProvider();

      //CEZ HDO code. Check your CEZ account/contract to get the code.
      var hdoCode = "A1B7dP2";

      //CEZ region (distribution area)
      var region = CezRegion.stred;

      //Call the GetScheduleAsync method
      var schedule = await provider.GetScheduleAsync(region, hdoCode);

      //Check if the HDO is now active.
      var isHdoActive = schedule.IsHdoActive();
      Console.WriteLine($"Is HDO active?: {isHdoActive}");

      //Check if the HDO will be active eight hours from now
      var timeToCheck = DateTime.Now.AddHours(8);
      var isHdoTime = schedule.IsHdoTime(timeToCheck);
      
      Console.WriteLine($"Will be HDO active at {timeToCheck.ToShortTimeString()}? : {isHdoTime}");
    }

    private static async Task checkRawJson()
    {
      //Create CEZ HDO provider
      using var provider = new CezHdoProvider();

      //CEZ HDO code. Check your CEZ account/contract to get the code.
      var hdoCode = "A1B7dP2";

      //CEZ region (distribution area)
      var region = CezRegion.stred;

      //Call the GetScheduleAsync method
      var schedule = await provider.GetScheduleAsync(region, hdoCode);

      //Write raw JSON response from CEZ web to console
      Console.WriteLine(schedule.AdditionalValues[CezHdoProvider.CEZ_FULL_JSON_RESPONSE_KEY]);

      //Get object model for raw JSON response
      var jsonRawObjectModel =
        (CezJsonRoot) schedule.AdditionalValues[CezHdoProvider.CEZ_FULL_OBJECT_MODEL_EQUIVALENT_TO_JSON_KEY];

      //And analyze data
      Console.WriteLine(jsonRawObjectModel.data[0].SAZBA);
      Console.WriteLine(jsonRawObjectModel.data[0].VALID_FROM);

      Console.ReadKey();
    }

    private static async Task runCachedHdoSchedule()
    {
      //HDO schedule is cached for one day from the time it is downloaded
      var scheduleValidForOneDay = TimeSpan.FromDays(1);
      using var cachedCezProvider = new CachedHdoProvider(new CezHdoProvider(), scheduleValidForOneDay);
      var innerProviderData = new Dictionary<string, string>()
      {
        {CezHdoProvider.AREA_KEY, CezRegion.stred.ToString()},
        {CezHdoProvider.HDO_CODE_KEY, "A1B7dP2"},
      };

      //Schedule is downloaded from CEZ web.
      var schedule = await cachedCezProvider.GetScheduleAsync(innerProviderData);

      Console.WriteLine(schedule);
      //schedule2 is returned immediately from the cache. No HTTP(S) request is made.
      var schedule2 = await cachedCezProvider.GetScheduleAsync(innerProviderData);
      Console.WriteLine(schedule2);
    }

    private static async Task getCezSchedule()
    {
      //Create CEZ HDO provider
      using var provider = new CezHdoProvider();

      //CEZ HDO code. Check your CEZ account/contract to get the code.
      var hdoCode = "A1B7dP2";

      //CEZ region (distribution area)
      var region = CezRegion.stred;

      //Call the GetScheduleAsync method
      var schedule = await provider.GetScheduleAsync(region, hdoCode);
      Console.Write(schedule.ToString());

    }
  }
}
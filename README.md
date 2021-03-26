# RStein.HDO
[![.NET](https://github.com/renestein/RStein.HDO/actions/workflows/dotnet.yml/badge.svg)](https://github.com/renestein/RStein.HDO/actions/workflows/dotnet.yml)

- C# API for 'HDO (Hromadne dalkove ovladani)'.
- .NET Standard 2.0 library. 
- Library is useful probably only for Czech citizens.
- Now supports only distribution regions served by the ČEZ company. PRs for other companies are welcome.

 # **Quick start**
 ## **Download HDO data from ČEZ company.**
``` C#
//Create CEZ HDO provider
using var provider = new CezHdoProvider();
//CEZ HDO code. Check your CEZ account/contract to get the code.
var hdoCode = "A1B7dP2";
//CEZ region (distribution area)
var region = CezRegion.stred;
//Call the GetScheduleAsync method
var schedule = await provider.GetScheduleAsync(region, hdoCode);
Console.Write(schedule.ToString());
```
Result:

![](https://snipboard.io/GuwQXY.jpg)

 ## **Cache the HDO schedule.**
 It is not wise to generate in a short period of time many HTTP(S) requests to HDO provider web/API because: 
 - HDO schedule is not changed frequently.
 - HDO provider may block your access to its website because many requests made in a short amount of time may be seen as the naive attempt of the denial-of-service (DoS) attack. We don't know in advance what definition of the 'short amount of time' the concrete HDO provider applies. Anyway, it is in our best interest to be good and mostly invisible users. :)
 ``` C#
//HDO schedule is cached for one day from the time it is downloaded
 var scheduleValidForOneDay = TimeSpan.FromDays(1);
 
 //Use CachedHdoProvider
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
 ```
 # **Test if HDO is active.**
- Use the method IsHdoActive on an instance of the HdoSchedule class to check if the HDO is active at the current time.
- Use the method IsHdoTime on an instance of the HdoSchedule class to check if the HDO is active at the given time.
 
``` C#
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
Console.WriteLine($"Will be HDO active at {timeToCheck.ToShortTimeString()}? : {isHdoTime}";
 ```
# **Use provider-specific data**
- Every HDO provider may publish additional unstructured information and provider-specific details in the AdditionalValues property (of type Dictionary<string, object>) on the instance of the HdoSchedule class.

- CezHdoProvider stores in the AdditionalValues property raw JSON response and corresponding raw object model.
 ``` C#
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
```


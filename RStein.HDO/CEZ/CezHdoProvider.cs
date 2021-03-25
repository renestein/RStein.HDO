using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RStein.HDO.Infrastructure;

namespace RStein.HDO.CEZ
{
  public class CezHdoProvider : IHdoScheduleProvider, IDisposable
  {
    public const string AREA_KEY = "Area";
    public const string HDO_CODE_KEY = "Hdo_Code";
    public const string PREDEFINED_FULL_API_URL_KEY = "Predefined_Url_API";
    public const string CEZ_FULL_OBJECT_MODEL_EQUIVALENT_TO_JSON_KEY = "Cez_Json_Object_Model";
    public const string CEZ_FULL_JSON_RESPONSE_KEY = "Cez_Json_Response";
    private const string UNEXPECTED_JSON_ERROR = "Unexpected json structure.";
    private const string UNKNOWN_ERROR= "Unexpected error occured. See inner exception for details.";
    private const char HOUR_MINUTE_SEPARATOR = ':';
    private const string APPLY_FROM_MONDAY_TO_FRIDAY_RAW_VALUE= "Po - Pá";
    private const string APPLY_FROM_SATURDAY_TO_SUNDAY_RAW_VALUE= "So - Ne";
    private const int INVALID_TIME_PART = -1;
    private const string UNEXPECTED_WEEK_DAYS_IN_PLAN_ERROR = "Unexpected week days (PLATNOST Json field) in plan.";
    public static readonly Uri DEFAULT_HDO_API_URL = new Uri("https://www.cezdistribuce.cz/distHdo/adam/containers/");

    private static readonly IEnumerable<DayOfWeek> FROM_MONDAY_TO_FRIDAY_VALIDITY = new[]
      {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday};
    
    private static readonly IEnumerable<DayOfWeek> FROM_SATURDAY_TO_SUNDAY_VALIDITY = new[] {DayOfWeek.Saturday, DayOfWeek.Sunday};

    private IHttpClient _httpClient;
    private readonly Uri _uri;
    private bool _ownsHttpClient;
    public CezHdoProvider() : this(new SimpleHttpClient(),
                                   DEFAULT_HDO_API_URL)
    {
      _ownsHttpClient = true;
    }

    public CezHdoProvider(IHttpClient  httpClient, Uri uri)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      _ownsHttpClient = false;
      _uri = uri ?? throw new ArgumentNullException(nameof(uri));
    }

    public string Name => nameof(CezHdoProvider);
    

    public async Task<HdoSchedule> GetScheduleAsync(CezRegion region,
                                                    string hdoCode)
    {
      if (region == CezRegion.Invalid)
      {
        throw new ArgumentException("region");
      }
      
      return await GetScheduleAsync(region.ToString(), hdoCode)
                  .ConfigureAwait(false);
    }

    public async Task<HdoSchedule> GetScheduleAsync(string region,
                                                    string hdoCode)
    {
      if (string.IsNullOrEmpty(region))
      {
        throw new ArgumentException($"'{nameof(region)}' cannot be null or empty", nameof(region));
      }

      if (string.IsNullOrEmpty(hdoCode))
      {
        throw new ArgumentException($"'{nameof(hdoCode)}' cannot be null or empty", nameof(hdoCode));
      }

      try
      {
        var apiUrl = prepareUrl(region, hdoCode);
        var scheduleJson = await downloadJson(apiUrl).ConfigureAwait(false);
        return parseJsonToHdoSchedule(scheduleJson);
      }
      catch (Exception ex)
      {
        throw new HdoException(UNKNOWN_ERROR, ex);
      }
    }

    public async Task<HdoSchedule> GetScheduleAsync(IDictionary<string, string> requestDetails)
    {
      if (requestDetails == null)
      {
        throw new ArgumentNullException(nameof(requestDetails));
      }

      try
      {
        
        if (!requestDetails.TryGetValue(PREDEFINED_FULL_API_URL_KEY, out var url))
        {
          return await GetScheduleAsync(requestDetails[AREA_KEY], requestDetails[HDO_CODE_KEY])
                      .ConfigureAwait(false);
        }

        var fullApiUrl = new Uri(url);
        var scheduleJson = await downloadJson(fullApiUrl).ConfigureAwait(false);
        return parseJsonToHdoSchedule(scheduleJson);
      }
      catch(Exception ex)
      {
        Debug.WriteLine(ex);
        throw new HdoException(UNKNOWN_ERROR, ex);
      }
    }
    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_ownsHttpClient && _httpClient != null)
        {
          (_httpClient as IDisposable)?.Dispose();
          _httpClient = null;
        }
      }
    }

    private HdoSchedule parseJsonToHdoSchedule(string scheduleJson)
    {
      var cezRoot = JObject.Parse(scheduleJson);
      var data = cezRoot["data"];

      if (data == null)
      {
        throw new HdoException(UNEXPECTED_JSON_ERROR);
      }

      var schedule = new HdoSchedule(nameof(CezHdoProvider),
                                     data.Children().Select(plan =>
                                                              new HdoScheduleIntervalDefinition(
                                                               Enumerable.Range(1, 10)
                                                                         .Select(timeIntervalIndex =>
                                                                         {
                                                                           var (beginHour, beginMinute) = parseHourAndMinute((string) plan[$"CAS_ZAP_{timeIntervalIndex}"]);
                                                                           if (beginHour == INVALID_TIME_PART)
                                                                           {
                                                                             return null;
                                                                           }

                                                                           var (endHour, endMinute) = parseHourAndMinute((string) plan[$"CAS_VYP_{timeIntervalIndex}"]);
                                                                           Debug.Assert(endHour != INVALID_TIME_PART);
                                                                           return new HdoScheduleIntervalItem(beginHour,
                                                                                                             beginMinute,
                                                                                                             endHour,
                                                                                                             endMinute);
                                                                         })
                                                                         .Where(item => item != null)
                                                                         .OrderBy(item => item.BeginHour)
                                                                         .ToArray(),
                                                               parseWeekDays((string) plan["PLATNOST"]),
                                                               getTimeFunc: null,
                                                               plan.ToObject<IDictionary<string, object>>()))
                                         .ToArray()
                                    );

      schedule.AdditionalValues.Add(CEZ_FULL_JSON_RESPONSE_KEY, scheduleJson);
      schedule.AdditionalValues.Add(CEZ_FULL_OBJECT_MODEL_EQUIVALENT_TO_JSON_KEY, JsonConvert.DeserializeObject<CezJsonRoot>(scheduleJson));

      return schedule;
    }

    private async Task<string> downloadJson(Uri apiUri)
    {
      var scheduleJson = await _httpClient.SendGetAsync(apiUri)
                                          .ConfigureAwait(false);
      return scheduleJson;
    }

    private IEnumerable<DayOfWeek> parseWeekDays(string rawWeekDays)
    {
      switch (rawWeekDays)
      {
        case APPLY_FROM_MONDAY_TO_FRIDAY_RAW_VALUE:
        {
          return FROM_MONDAY_TO_FRIDAY_VALIDITY;
        }
        case APPLY_FROM_SATURDAY_TO_SUNDAY_RAW_VALUE:
        {
          return FROM_SATURDAY_TO_SUNDAY_VALIDITY;
        }
        default:
        {
          throw new HdoException(UNEXPECTED_WEEK_DAYS_IN_PLAN_ERROR);
        }
      }
      
    }

    private (int hour, int minute) parseHourAndMinute(string rawHourAndMinutes)
    {
      const int HOUR_INDEX = 0;
      const int MINUTE_INDEX = 1;
      if (rawHourAndMinutes == null)
      {
        return (INVALID_TIME_PART, INVALID_TIME_PART);
      }
      
      var hoursAndMinutes = rawHourAndMinutes.Split(HOUR_MINUTE_SEPARATOR);
      return (int.Parse(hoursAndMinutes[HOUR_INDEX]), int.Parse(hoursAndMinutes[MINUTE_INDEX]));
    }

    private Uri prepareUrl(string region, string hdoCode)
    {
      var uriBuilder = new UriBuilder(_uri);
      uriBuilder.Path = uriBuilder.Path + region;
      uriBuilder.Query = $"code={hdoCode.ToUpperInvariant()}";
      return uriBuilder.Uri;
    }

    private static CezHdoProvider createDefaultProvider()
    {
      return new CezHdoProvider(new SimpleHttpClient(), DEFAULT_HDO_API_URL);
    }
  }
}
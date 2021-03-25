#pragma warning disable ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RStein.HDO.CEZ;
using RStein.HDO.Infrastructure;
using RStein.HDO.Test.Stubs;

namespace RStein.HDO.Test.Cez
{
  public class CezHdoProviderTest
  {
    public const string TEST_DATA_DIRECTORY = "TestData";
    public const string VALID_CEZ_JSON = "Cez_ValidResponse.txt";
    public const string EMPTY_DATA_RESPONSE_CEZ_JSON = "Cez_EmptyDataResponse.txt";
    private const string TEST_HDO_CODE = "A1B7dP2";

    [Test]
    public async Task GetScheduleAsync_When_Valid_Json_Then_Returns_Expected_Schedule()
    {
      var expectedSchedule = getExpectedValidSchedule();

      var validJson = getValidCezJsonResponse();
      HdoSchedule schedule;
      using (var provider = new CezHdoProvider(new SimpleHttpClientStub(validJson), CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        schedule = await provider.GetScheduleAsync(new Dictionary<string, string> {[CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE, [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString()});
      }

      Assert.AreEqual(expectedSchedule, schedule);

    }

    [Test]
    public async Task GetScheduleAsync_CezRegion_String_When_Valid_Json_Then_Returns_Expected_Schedule()
    {
      var expectedSchedule = getExpectedValidSchedule();

      var validJson = getValidCezJsonResponse();
      HdoSchedule schedule;
      using (var provider = new CezHdoProvider(new SimpleHttpClientStub(validJson), CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        schedule = await provider.GetScheduleAsync(CezRegion.stred, TEST_HDO_CODE);
      }

      Assert.AreEqual(expectedSchedule, schedule);
    }

    [Test]
    public async Task GetScheduleAsync_String_String_When_Valid_Json_Then_Returns_Expected_Schedule()
    {
      var expectedSchedule = getExpectedValidSchedule();

      var validJson = getValidCezJsonResponse();
      HdoSchedule schedule;
      using (var provider = new CezHdoProvider(new SimpleHttpClientStub(validJson), CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        schedule = await provider.GetScheduleAsync(CezRegion.stred.ToString(), TEST_HDO_CODE);
      }

      Assert.AreEqual(expectedSchedule, schedule);
    }
    
    [Test]
    public async Task GetScheduleAsync_When_Invalid_Json_Then_Returns_Empty_Schedule()
    {
      var expectedEmptyCezSchedule = new HdoSchedule(nameof(CezHdoProvider), Enumerable.Empty<HdoScheduleIntervalDefinition>());
      var invalidJson = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory,
                                                            TEST_DATA_DIRECTORY, EMPTY_DATA_RESPONSE_CEZ_JSON));
      HdoSchedule schedule;
      using (var provider = new CezHdoProvider(new SimpleHttpClientStub(invalidJson), CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        schedule = await provider.GetScheduleAsync(new Dictionary<string, string> {[CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE, [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString()});
      }

      Assert.AreEqual(expectedEmptyCezSchedule, schedule);
    }

    [Test]
    public void GetScheduleAsync_When_Empty_Response_Then_Throws_HdoException()
    {
      var emptyJson = String.Empty;
      var provider = new CezHdoProvider(new SimpleHttpClientStub(emptyJson), CezHdoProvider.DEFAULT_HDO_API_URL);

      Assert.CatchAsync<HdoException>(async () =>
      {
        var _ = await provider.GetScheduleAsync(new Dictionary<string, string>
        {
          [CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE,
          [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString()

        });
      });
    }

    [Test]
    public void GetScheduleAsync_When_Request_Details_Is_Null_Then_Throws_ArgumentNullException()
    {
      var emptyJson = String.Empty;
      var provider = new CezHdoProvider(new SimpleHttpClientStub(emptyJson), CezHdoProvider.DEFAULT_HDO_API_URL);

      Dictionary<string, string> nullRequestDetail = null;
      Assert.CatchAsync<ArgumentNullException>(async () =>
      {
        var _ = await provider.GetScheduleAsync(nullRequestDetail);
      });
    }
    
    [Test]
    public void GetScheduleAsync_When_HdoCode_Missing_And_Full_Request_Url_Not_Provided_Then_Throws_HdoException()
    {
      var emptyJson = String.Empty;
      var provider = new CezHdoProvider(new SimpleHttpClientStub(emptyJson), CezHdoProvider.DEFAULT_HDO_API_URL);

      Assert.CatchAsync<HdoException>(async () =>
      {
        var _ = await provider.GetScheduleAsync(new Dictionary<string, string>
        {
          [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString()

        });
      });
    }

    [Test]
    public async Task GetScheduleAsync_When_Full_Request_Url_Provided_Then_Provider_Uses_Full_Request_Url()
    {
      var expectedRequestUri = new Uri("https://example.com");
      var validJson = getValidCezJsonResponse();
      var httpClientMock = Substitute.For<IHttpClient>();
      httpClientMock.SendGetAsync(Arg.Any<Uri>()).Returns(validJson);

      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        var _ = await provider.GetScheduleAsync(new Dictionary<string, string>
        {
          [CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE,
          [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString(),
          [CezHdoProvider.PREDEFINED_FULL_API_URL_KEY] = expectedRequestUri.ToString()
        
        });
      }

      await httpClientMock.Received().SendGetAsync(expectedRequestUri);
    }

    [Test]
    public async Task GetScheduleAsync_When_Full_Request_Url_Not_Provided_Then_Provider_Uses_Default_Url()
    {
      var validJson = getValidCezJsonResponse();
      var httpClientMock = Substitute.For<IHttpClient>();
      httpClientMock.SendGetAsync(Arg.Any<Uri>()).Returns(validJson);

      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        var _ = await provider.GetScheduleAsync(new Dictionary<string, string>
        {
          [CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE,
          [CezHdoProvider.AREA_KEY] = CezRegion.stred.ToString()
        });
      }

      await httpClientMock.Received().SendGetAsync(Arg.Is<Uri>(uri => uri.ToString().StartsWith(CezHdoProvider.DEFAULT_HDO_API_URL.ToString())));
    }

    [Test]
    public void GetScheduleAsync_When_Area_Is_Missing_And_Full_Request_Url_Not_Provided_Then_Throws_HdoException()
    {
      var emptyJson = String.Empty;
      var provider = new CezHdoProvider(new SimpleHttpClientStub(emptyJson), CezHdoProvider.DEFAULT_HDO_API_URL);

      Assert.CatchAsync<HdoException>(async () =>
      {
        var _ = await provider.GetScheduleAsync(new Dictionary<string, string>
        {
          [CezHdoProvider.HDO_CODE_KEY] = TEST_HDO_CODE,

        });
      });
    }
    
    [Test]
    public void Ctor_When_HttpClient_Is_Null_Then_Throws_ArgumentNullException()
    {
      IHttpClient nullHttpClient = null;
      Assert.Catch<ArgumentNullException>(() =>
      {
        var _ = new CezHdoProvider(nullHttpClient, CezHdoProvider.DEFAULT_HDO_API_URL);
      });
    }

    [Test]
    public void Ctor_When_Request_Uri_Is_Null_Then_Throws_ArgumentNullException()
    {
      Uri nullUri = null;
      
      Assert.Catch<ArgumentNullException>(() =>
      {
        var _ = new CezHdoProvider(new SimpleHttpClientStub(String.Empty), nullUri);
      });
    }

    [Test]
    public void Name_When_Called_Then_Returns_Expected_Name()
    {
      const string EXPECTED_PROVIDER_NAME = "CezHdoProvider"; //Do not use nameof!

      var httpClientMock = Substitute.For<IHttpClient>();
      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        Assert.AreEqual(EXPECTED_PROVIDER_NAME, provider.Name);
      }
    }

    [Test]
    public void GetScheduleAsync_When_Region_Is_Invalid_Then_Throws_ArgumentException()
    {
      var httpClientMock = Substitute.For<IHttpClient>();
      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        Assert.CatchAsync<ArgumentException>(async () => await provider.GetScheduleAsync(CezRegion.Invalid, TEST_HDO_CODE));
      }
    }

    [TestCase(null)]
    [TestCase("")]
    public void GetScheduleAsync_When_Hdo_Code_Is_Invalid_Then_Throws_ArgumentException(string invalidHdoCode)
    {
      var httpClientMock = Substitute.For<IHttpClient>();
      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        Assert.CatchAsync<ArgumentException>(async () => await provider.GetScheduleAsync(CezRegion.stred, invalidHdoCode));
      }
    }

    [TestCase(null)]
    [TestCase("")]
    public void GetScheduleAsync_String_string_When_Hdo_Code_Is_Invalid_Then_Throws_ArgumentException(string invalidHdoCode)
    {
      var httpClientMock = Substitute.For<IHttpClient>();
      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        Assert.CatchAsync<ArgumentException>(async () => await provider.GetScheduleAsync("stred", invalidHdoCode));
      }
    }

    [TestCase(null)]
    [TestCase("")]
    public void GetScheduleAsync_String_string_When_Region_Is_Invalid_Then_Throws_ArgumentException(string invalidRegion)
    {
      var httpClientMock = Substitute.For<IHttpClient>();
      using (var provider = new CezHdoProvider(httpClientMock, CezHdoProvider.DEFAULT_HDO_API_URL))
      {
        Assert.CatchAsync<ArgumentException>(async () => await provider.GetScheduleAsync(invalidRegion, TEST_HDO_CODE));
      }
    }

    private static string getValidCezJsonResponse()
    {
      return File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TEST_DATA_DIRECTORY, VALID_CEZ_JSON));
    }

    private static HdoSchedule getExpectedValidSchedule()
    {
      return new HdoSchedule(nameof(CezHdoProvider), new[]
      {
        new HdoScheduleIntervalDefinition(new List<HdoScheduleIntervalItem>
        {
          new HdoScheduleIntervalItem(0, 0, 7, 55),
          new HdoScheduleIntervalItem(8, 50, 11, 55),
          new HdoScheduleIntervalItem(12, 50, 14, 35),
          new HdoScheduleIntervalItem(15, 35, 19, 50),
          new HdoScheduleIntervalItem(20, 50, 23, 59)
        }, new[] {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday}),

        new HdoScheduleIntervalDefinition(new List<HdoScheduleIntervalItem>
        {
          new HdoScheduleIntervalItem(0, 0, 7, 55),
          new HdoScheduleIntervalItem(8, 50, 10, 55),
          new HdoScheduleIntervalItem(11, 50, 14, 10),
          new HdoScheduleIntervalItem(15, 10, 19, 15),
          new HdoScheduleIntervalItem(20, 15, 23, 59),

        }, new[] {DayOfWeek.Saturday, DayOfWeek.Sunday})

      });
    }
  }
}

#pragma warning restore ConfigureAwaitEnforcer // ConfigureAwaitEnforcer

#pragma warning disable ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace RStein.HDO.Test
{
  [TestFixture]
  public class CachedHdoProviderTest
  {
    public const string TEST_PROVIDER_NAME = "HDO Test provider";
    [Test]
    public void Ctor_When_Inner_Provider_Is_Null_Then_Throws_ArgumentNullException()
    {
      IHdoScheduleProvider nullHdoProvider = null;
      var validFor = TimeSpan.FromHours(1);
      Assert.Catch<ArgumentNullException>(() =>
      {
        var _ = new CachedHdoProvider(nullHdoProvider, validFor);
      });
    }

    [Test]
    public async Task GetScheduleAsync_When_First_Call_Then_Returns_Data_From_Underlying_ProviderAsync()
    {
      
      IHdoScheduleProvider innerProvider = Substitute.For<IHdoScheduleProvider>();
      var expectedHdoSchedule = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      innerProvider.GetScheduleAsync(Arg.Any<IDictionary<string, string>>())
                   .Returns(expectedHdoSchedule);
      var validFor = TimeSpan.FromHours(1);
      var now = DateTime.Now;
      HdoSchedule hdoSchedule;
      using (var cachedHdoProvider = new CachedHdoProvider(innerProvider, validFor, () => now))
      {
        hdoSchedule = await cachedHdoProvider.GetScheduleAsync(new Dictionary<string, string>());
      }

      Assert.AreSame(expectedHdoSchedule, hdoSchedule);
    }

    
    [Test]
    public async Task GetScheduleAsync_When_Second_Call_And_Not_Expired_Then_Returns_Same_Schedule()
    {
      
      IHdoScheduleProvider innerProvider = Substitute.For<IHdoScheduleProvider>();
      var expectedHdoSchedule = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      innerProvider.GetScheduleAsync(Arg.Any<IDictionary<string, string>>())
                   .Returns(expectedHdoSchedule);
      var validFor = TimeSpan.FromHours(1);
      var now = DateTime.Now;
      var secondCallTime = now.AddSeconds(1);
      var isFirstCall = true;
      HdoSchedule hdoSchedule2;
      using (var cachedHdoProvider = new CachedHdoProvider(innerProvider, validFor, () => isFirstCall
                                                             ? now
                                                             : secondCallTime))
      {
        var _ = await cachedHdoProvider.GetScheduleAsync(new Dictionary<string, string>());
      
        isFirstCall = false;
      
        hdoSchedule2 = await cachedHdoProvider.GetScheduleAsync(new Dictionary<string, string>());
      }

      Assert.AreSame(expectedHdoSchedule, hdoSchedule2);
    }

    [TestCaseSource("generateCacheTimeSpans")]
    public async Task GetScheduleAsync_When_Second_Call_And_Expired_Then_Returns_New_Schedule(TimeSpan addToCurrentExpiredDate)
    {
      
      IHdoScheduleProvider innerProvider = Substitute.For<IHdoScheduleProvider>();
      var firstHdoSchedule = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      var newScheduleAfterExpiration = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      innerProvider.GetScheduleAsync(Arg.Any<IDictionary<string, string>>())
                   .Returns(firstHdoSchedule, newScheduleAfterExpiration);
      var validFor = TimeSpan.FromHours(1);
      var now = DateTime.Now;

      var timeAfterExpiration = now + validFor + addToCurrentExpiredDate;
      var isFirstCall = true;
      HdoSchedule hdoSchedule;
      using (var cachedHdoProvider = new CachedHdoProvider(innerProvider, validFor, () => isFirstCall ? now : timeAfterExpiration))
      {
        var _ = await cachedHdoProvider.GetScheduleAsync(new Dictionary<string, string>());
        isFirstCall = false;

        hdoSchedule = await cachedHdoProvider.GetScheduleAsync(new Dictionary<string, string>());
      }

      Assert.AreSame(newScheduleAfterExpiration, hdoSchedule);
    }

    [Test]
    public async Task GetScheduleAsync_When_Using_Different_Args_Then_Returns_Different_Schedule()
    {
      
      IHdoScheduleProvider innerProvider = Substitute.For<IHdoScheduleProvider>();
      var expectedHdoSchedule = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());
      var expectedHdoSchedule2 = new HdoSchedule(TEST_PROVIDER_NAME, Enumerable.Empty<HdoScheduleIntervalDefinition>());

      var firstDictionary = new Dictionary<string, string>
      {
        ["key1"] = "value1"
      };
      
      var secondDictionary = new Dictionary<string, string>
      {
        ["key1"] = "value1",
        ["key2"] = "value2"
      };
      
      innerProvider.GetScheduleAsync(firstDictionary)
                   .Returns(expectedHdoSchedule);

      innerProvider.GetScheduleAsync(secondDictionary)
                   .Returns(expectedHdoSchedule2);

      var validFor = TimeSpan.FromHours(1);
      var now = DateTime.Now;
      HdoSchedule hdoSchedule;
      HdoSchedule hdoSchedule2;
      HdoSchedule secondCallHdoSchedule;
      HdoSchedule secondCallHdoSchedule2;
      using (var cachedHdoProvider = new CachedHdoProvider(innerProvider, validFor))
      {
        hdoSchedule = await cachedHdoProvider.GetScheduleAsync(firstDictionary);
        hdoSchedule2 = await cachedHdoProvider.GetScheduleAsync(secondDictionary);
        secondCallHdoSchedule = await cachedHdoProvider.GetScheduleAsync(firstDictionary);
        secondCallHdoSchedule2 = await cachedHdoProvider.GetScheduleAsync(secondDictionary);
      }

      Assert.AreSame(expectedHdoSchedule, hdoSchedule);
      Assert.AreSame(expectedHdoSchedule2, hdoSchedule2);
      Assert.AreSame(expectedHdoSchedule, secondCallHdoSchedule);
      Assert.AreSame(expectedHdoSchedule2, secondCallHdoSchedule2);
    }
    private static IEnumerable<TimeSpan> generateCacheTimeSpans()
    {
      yield return TimeSpan.FromSeconds(1);
      yield return TimeSpan.FromSeconds(2);
      yield return TimeSpan.FromMinutes(1);
      yield return TimeSpan.FromMinutes(2);
      yield return TimeSpan.FromMinutes(10);
      yield return TimeSpan.FromMinutes(40);
      yield return TimeSpan.FromHours(1);
      yield return TimeSpan.FromHours(9);
      yield return TimeSpan.FromDays(1);
    }
  }
}
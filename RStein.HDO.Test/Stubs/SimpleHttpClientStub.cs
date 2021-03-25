using System;
using System.Threading.Tasks;
using RStein.HDO.Infrastructure;

namespace RStein.HDO.Test.Stubs
{
  public class SimpleHttpClientStub : IHttpClient
  {
    private readonly string _retJson;

    public SimpleHttpClientStub(string retJson)
    {
      _retJson = retJson;
    }

    public Task<string> SendGetAsync(Uri uri)
    {
      return Task.FromResult(_retJson);
    }
  }
}
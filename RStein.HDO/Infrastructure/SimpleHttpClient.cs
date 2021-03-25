using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RStein.HDO.Infrastructure
{
  public class SimpleHttpClient : IHttpClient, IDisposable
  {
    private HttpClient _httpClient;
    public SimpleHttpClient()
    {
      _httpClient = new HttpClient();
    }

    public async Task<string> SendGetAsync(Uri uri)
    {
      if (uri == null)
      {
        throw new ArgumentNullException(nameof(uri));
      }

      return await _httpClient.GetStringAsync(uri)
                              .ConfigureAwait(false);
    }

    public void Dispose()
    {
      _httpClient?.Dispose();
      _httpClient = null;
    }
  }
}
using System;
using System.Threading.Tasks;

namespace RStein.HDO.Infrastructure
{
  public interface IHttpClient
  {
    Task<string> SendGetAsync(Uri uri);
  }
}
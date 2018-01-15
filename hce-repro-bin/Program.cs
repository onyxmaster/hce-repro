using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace hce_repro_bin
{
    class Program
    {
        private static readonly Uri RequestUri = new Uri("https://www.amazon.com/", UriKind.Absolute);

        private const int DelayMs = 300;

        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            Parallel.For(1, 100000, 
                () => new Random(), 
                (_1, _2, rng) => { Request(httpClient, rng.Next(DelayMs)).GetAwaiter().GetResult(); return rng; }, 
                _ => { });
        }

        private static async Task Request(HttpClient httpClient, int delay)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, RequestUri);
            var cts = new CancellationTokenSource(delay);
            try
            {
                using (await httpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                {
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulkTMDBDownloader
{
    class RequestManager
    {
        private const int Ratelimit = 40; //(requests per rate time) 
        private const int Ratetime = 10000; //(ms)
        private readonly Queue<Query> _requestQueue;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _requestHandlingLoop;

        public RequestManager()
        {
            _requestQueue = new Queue<Query>();
            _cancellationTokenSource = new CancellationTokenSource();
            _requestHandlingLoop = new Task(StartRequestHandlingLoop, _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning);
            _requestHandlingLoop.Start();
        }

        /// <summary>
        /// Starts the request handling loop.
        /// </summary>
        private async void StartRequestHandlingLoop()
        {
            Stopwatch stopWatch = new Stopwatch();
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                stopWatch.Reset();
                stopWatch.Start();
                await ExecuteRequest();
                int elapsedTime = (int)stopWatch.ElapsedMilliseconds;

                if(elapsedTime < Ratetime / Ratelimit)
                    _requestHandlingLoop.Wait(Ratetime/Ratelimit - elapsedTime);
            }
            stopWatch.Stop();
        }

        /// <summary>
        /// Add a request to the request queue
        /// </summary>
        /// <param name="query">The query to be requested</param>
        public void AddRequest(Query query)
        {
            _requestQueue.Enqueue(query);
        }

        /// <summary>
        /// Take one request of the queue and execute it
        /// </summary>
        /// <returns>It calls the callback method</returns>
        private async Task ExecuteRequest()
        {
            if (_requestQueue.Count == 0)
                return;

            Query query = _requestQueue.Dequeue();
            Uri uri = new Uri(Uri.EscapeUriString(query.GetUrl()));

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    query.CallbackAction(await httpClient.GetStringAsync(uri));
                    if (query.RequestType == Query.RequestTypes.Search)
                        Program.SearchCount++;
                    else
                        Program.GetCount++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Something went wrong: {e.Message}");
                }
            }
        }
    }
}

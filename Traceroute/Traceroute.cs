using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Traceroute
{
    public class Traceroute
    {
        private static Random _random = new Random();

        public static async IAsyncEnumerable<TracerouteHop> GetTraceRouteAsync(string hostname, int timeout = 5000, int pings = 5, int maxTTL = 30, CancellationToken cancellationToken = default(CancellationToken))
        {
            const int bufferSize = 32;

            byte[] buffer = new byte[bufferSize];

            // Generate random bytes
            _random.NextBytes(buffer);

            using (var pinger = new Ping())
            {
                var sw = Stopwatch.StartNew();

                for (int ttl = 1; ttl <= maxTTL; ttl++)
                {
                    long totalTimeElapsed = 0;
                    int successes = 0;
                    int failures = 0;
                    var results = new List<int>();

                    
                    PingReply reply = null;
                    for (var p = 0; p < pings; p++)
                    {
                        sw.Restart();
                        PingOptions options = new PingOptions(ttl, true);
                        reply = await pinger.SendPingAsync(hostname, timeout, buffer, options);

                        // if we reach a status other than expired or timed out, we're done searching or there has been an error
                        if (cancellationToken.IsCancellationRequested || (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut))
                        {
                            ttl += int.MaxValue; // Break the other loop as well
                            reply = null;
                            break;
                        }


                        if (reply.Status != IPStatus.Success || reply.Status == IPStatus.TtlExpired)
                        {
                            results.Add((int)sw.ElapsedMilliseconds);
                            successes++;
                        }
                        else
                            failures++;
                    }

                    totalTimeElapsed = sw.ElapsedMilliseconds;

                    if (reply != null)
                    {
                        string ipHost = "";
                        try
                        {
                            var hostInfo = Dns.GetHostEntry(reply.Address.ToString());
                            ipHost = hostInfo.HostName;
                        }
                        catch { }

                        yield return new TracerouteHop()
                        {
                            Hostname = ipHost,
                            IPAddress = reply.Address,
                            SuccessfulPings = successes,
                            FailedPings = failures,
                            Index = ttl,
                            SentPings = pings,
                            AveragePing = results.Count == 0 ? 0 : (int)(sw.ElapsedMilliseconds / (double)results.Sum()),
                            SlowestPing = results.Count == 0 ? 0 : results.OrderByDescending(e => e).First(),
                            FastestPing = results.Count == 0 ? 0 : results.OrderBy(e => e).First(),
                        };
                    }
                }
            }
        }

        public static string GetHeader()
        {
            return $"{"Host",-64} {"Loss %",10} {"Fastest",10} {"Slowest",10} {"Average",10}";
        }
    }
}

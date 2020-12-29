using System;
using System.Threading.Tasks;

namespace Traceroute.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Console.WriteLine(Traceroute.GetHeader());
                await foreach (var hop in Traceroute.GetTraceRouteAsync("google.com"))
                {
                    Console.WriteLine(hop.ToString());
                }
            });

            Console.ReadLine();
        }
    }
}

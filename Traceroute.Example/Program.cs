using System;
using System.Threading.Tasks;
using System.Net;

namespace TracerouteExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Console.WriteLine(Traceroute.GetHeader());
                foreach (var hop in Traceroute.GetTraceRoute("google.com"))
                {
                    Console.WriteLine(hop.ToString());
                }
            });

            Console.ReadLine();
        }
    }
}

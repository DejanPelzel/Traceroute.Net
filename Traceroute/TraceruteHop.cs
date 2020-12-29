using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace System.Net
{
    public struct TracerouteHop
    {
        public int Index { get; set; }
        public string Hostname { get; set; }
        public IPAddress IPAddress { get; set; }
        public int SuccessfulPings { get; set; }
        public int FailedPings { get; set; }
        public int SentPings { get; set; }
        public int FastestPing { get; set; }
        public int SlowestPing { get; set; }
        public int AveragePing { get; set; }

        public int PacketLoss
        {
            get
            {
                return (int)((FailedPings / (double)SuccessfulPings) * 100);
            }
        }

        public override string ToString()
        {
            var host = this.Hostname;
            if (host.Length > 40)
                host = host.Remove(40) + "...";

            host += $" ({this.IPAddress.ToString()})";

            host = host.TrimStart();
            if (host.StartsWith("("))
                host = host.Remove(host.Length - 1).Remove(0, 1);

            return $"{this.Index,2}. {host,-60} {this.PacketLoss.ToString() + " %",10} {this.FastestPing + " ms",10} {this.SlowestPing + " ms",10} {this.AveragePing + " ms",10}";
        }
    }
}

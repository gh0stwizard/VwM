using System;
using System.Collections.Generic;
using System.Text;

namespace VwM.BackgroundServices.Ping
{
    public class PingDto
    {
        public string Name { get; private set; }
        public string Hostname { get; set; }


        public PingDto(string name, string hostname)
        {
            Name = name;
            Hostname = hostname;
        }

        public PingDto(string hostname) : this(hostname, hostname) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VwM.BackgroundServices.Whois
{
    public class WhoisDto
    {
        public string Hostname { get; set; }


        public WhoisDto(string hostname)
        {
            Hostname = hostname;
        }
    }
}

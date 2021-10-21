using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpRestApi.Models
{
    public class IpAddress
    {
        public string ip { get; set; }

        public bool isAvailable { get; set; }

        public IpAddress(string ip, bool isAvailable)
        {
            this.ip = ip;
            this.isAvailable = isAvailable;
        }

        public IpAddress()
        {
            ip = "";
            isAvailable = false;
        }

    }
}

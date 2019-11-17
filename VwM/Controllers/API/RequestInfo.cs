using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VwM.Controllers.API
{
    public class RequestInfo
    {
        private const int MaxTake = 1000;
        private int _take = 0;
        private int _page = 1;


        public int Take
        {
            get { return _take; }
            internal set
            {
                if (value > 0 && value < MaxTake)
                    _take = value;
            }
        }


        public int Skip
        {
            get { return (Page - 1) * Take; }
        }


        public int Page
        {
            get { return _page; }
            internal set
            {
                if (value > 0)
                    _page = value;
            }
        }


        public string Search { get; internal set; } = "";
        public bool UseSelect2 { get; internal set; }
    }
}

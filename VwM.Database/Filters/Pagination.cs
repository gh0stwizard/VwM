using System;
using System.Collections.Generic;
using System.Text;

namespace VwM.Database.Filters
{
    public class Pagination
    {
        private const int MaxTake = 1000;
        private int _take = 0;
        private int _page = 1;


        public int Take
        {
            get { return _take; }
            set
            {
                if (value > 0 && value < MaxTake)
                    _take = value;
            }
        }


        public int Skip
        {
            get { return (Page - 1) * Take; }
            set
            {
                if (value > 0)
                    Page = (value / Take) + 1;
            }
        }


        public int Page
        {
            get { return _page; }
            set
            {
                if (value > 0)
                    _page = value;
            }
        }
    }
}

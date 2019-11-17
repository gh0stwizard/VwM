using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VwM.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetLastException(this Exception e)
        {
            var last = e;
            while (last.InnerException != null)
                last = last.InnerException;
            return last;
        }
    }
}

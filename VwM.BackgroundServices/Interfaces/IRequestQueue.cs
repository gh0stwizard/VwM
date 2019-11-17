using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VwM.BackgroundServices
{
    public interface IRequestQueue
    {
        void Cleanup();
    }
}

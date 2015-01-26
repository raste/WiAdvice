using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer
{
    public class Stopwatch
    {
        DateTime startTime = DateTime.UtcNow;

        public TimeSpan Duration
        {
            get
            {
                DateTime currentTime = DateTime.UtcNow;
                TimeSpan interval = currentTime - startTime;
                return interval;
            }
        }
    }
}

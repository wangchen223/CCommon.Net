using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.RateLimiter
{
    //public abstract class SleepingStopwatch
    public class SleepingStopwatch
    {
        readonly Stopwatch stopwatch = new Stopwatch();
        public SleepingStopwatch()
        {
            stopwatch.Start();
        }



        public static SleepingStopwatch createFromSystemTimer()
        {
            return new SleepingStopwatch();
        }
        public long readMicros()
        {
            return stopwatch.Elapsed.Ticks;
//            return Convert.ToInt64(stopwatch.Elapsed.TotalMilliseconds*100);
//            return stopwatch.elapsed(TimeUnit.MICROSECONDS);
        }


        public void sleepMicrosUninterruptibly(long micros)
        {
            if (micros > 0)
            {
                System.Threading.Thread.Sleep((int)(micros/10000));
                //Uninterruptibles.sleepUninterruptibly(micros, TimeUnit.MICROSECONDS);
            }
        }
    }
}

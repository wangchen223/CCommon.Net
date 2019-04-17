using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CCommon.Common.RateLimiter
{
    public abstract class SmoothRateLimiter : RateLimiter
    {
        private volatile Object mutexDoNotUseDirectly;
        private Object mutex()
        {
            Object mutex = mutexDoNotUseDirectly;
            if (mutex == null)
            {
                lock (this)
                {
                    mutex = mutexDoNotUseDirectly;
                    if (mutex == null)
                    {
                        mutexDoNotUseDirectly = mutex = new Object();
                    }
                }
            }
            return mutex;
        }


        /** The currently stored permits. */
        protected double storedPermits;

        /** The maximum number of stored permits. */
        protected double maxPermits;

        /**
         * The interval between two unit requests, at our stable rate. E.g., a stable rate of 5 permits
         * per second has a stable interval of 200ms.
         */
        protected double stableIntervalMicros;

        /**
         * The time when the next request (no matter its size) will be granted. After granting a request,
         * this is pushed further in the future. Large requests push this further than small requests.
         */
        private long nextFreeTicketMicros = 0L; // could be either in the past or future

        protected SmoothRateLimiter(SleepingStopwatch stopwatch) : base(stopwatch)
        {
        }


        protected override void doSetRate(double permitsPerSecond, long nowMicros)
        {
            resync(nowMicros);
            double stableIntervalMicros = TimeUnit.Seconds.toMicros(1L) / permitsPerSecond;
            this.stableIntervalMicros = stableIntervalMicros;
            doSetRate(permitsPerSecond, stableIntervalMicros);
        }

        protected abstract void doSetRate(double permitsPerSecond, double stableIntervalMicros);


        protected override double doGetRate()
        {
            return TimeUnit.Seconds.toMicros(1L) / stableIntervalMicros;
        }


        protected override long queryEarliestAvailable(long nowMicros)
        {
            return nextFreeTicketMicros;
        }


        protected override long reserveEarliestAvailable(int requiredPermits, long nowMicros)
        {
            //重新计算桶内令牌数storedPermits
            lock (mutex())
            {
                resync(nowMicros);
            }

            lock (mutex())
            {
                long returnValue = nextFreeTicketMicros;//下一次请求可以获取令牌的起始时间
                                                        //本次消耗的令牌数
                double storedPermitsToSpend = Math.Min(requiredPermits, this.storedPermits);//本次消耗的令牌数=min(申请令牌数,当前存储令牌数)
                #region 预支逻辑                                                                            //重新计算下次可获取时间nextFreeTicketMicros
                double freshPermits = requiredPermits - storedPermitsToSpend;//缺少令牌数=申请令牌数-本次可以消耗的令牌数
                long waitMicros =
                    storedPermitsToWaitTime(this.storedPermits, storedPermitsToSpend)
                        + (long)(freshPermits * stableIntervalMicros);//缺少令牌数*生成令牌的时间间隔(提前预支)

                this.nextFreeTicketMicros = LongHelper.SaturatedAdd(nextFreeTicketMicros, waitMicros);//防止溢出 下一次分配时间+预支分配令牌的时间
                #endregion                                                                      //Console.WriteLine("nextFreeTicketMicros:" + this.nextFreeTicketMicros+"_"+ waitMicros);
                this.storedPermits -= storedPermitsToSpend;
                return returnValue;
            }
        }
        

        /**
         * Translates a specified portion of our currently stored permits which we want to spend/acquire,
         * into a throttling time. Conceptually, this evaluates the integral of the underlying function we
         * use, for the range of [(storedPermits - permitsToTake), storedPermits].
         *
         * <p>This always holds: {@code 0 <= permitsToTake <= storedPermits}
         */
        protected abstract long storedPermitsToWaitTime(double storedPermits, double permitsToTake);

        /**
         * Returns the number of microseconds during cool down that we have to wait to get a new permit.
         */
        protected abstract double coolDownIntervalMicros();


        /// <summary>
        /// Updates {@code storedPermits} and {@code nextFreeTicketMicros} based on the current time.
        /// 该函数会在每次获取令牌之前调用，其实现思路为，若当前时间晚于nextFreeTicketMicros，
        /// 则计算该段时间内可以生成多少令牌，将生成的令牌加入令牌桶中并更新数据。这样一来，只需要在获取令牌时计算一次即可。
        /// </summary>
        /// <param name="nowMicros"></param>
        void resync(long nowMicros)
        {
            // if nextFreeTicket is in the past, resync to now            
            if (nowMicros > nextFreeTicketMicros)
            {
                Console.WriteLine(string.Format("resync()_nextFreeTicketMicros:{1}_nowMicros:{0}", nowMicros, nextFreeTicketMicros));
                
                double newPermits = (nowMicros - nextFreeTicketMicros) / coolDownIntervalMicros();//可以生成的令牌数 当前时间-下一次分配时间/平均每秒可令牌数(不能大于最大令牌数)
                storedPermits = Math.Min(maxPermits, storedPermits + newPermits);//当前存储令牌数=min(最大存储令牌数,可生成的令牌数);
                nextFreeTicketMicros = nowMicros;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.RateLimiter
{
    public class SmoothBursty: SmoothRateLimiter
    {
        /** The work (permits) of how many seconds can be saved up if this RateLimiter is unused?
     * 在RateLimiter未使用时，最多存储几秒的令牌
     * */
        readonly double maxBurstSeconds;

        public SmoothBursty(SleepingStopwatch stopwatch, double maxBurstSeconds):base(stopwatch)
        {
            this.maxBurstSeconds = maxBurstSeconds;
        }

        /**
         * 重设流量相关参数，需要子类来实现，不同子类参数不尽相同，比如SmoothWarmingUp肯定有增长比率相关参数
         * 初始化速率
         * */

        protected override void doSetRate(double permitsPerSecond, double stableIntervalMicros)
        {
            double oldMaxPermits = this.maxPermits;
            //最大令牌数
            maxPermits = maxBurstSeconds * permitsPerSecond;//最大突发令牌数*每秒令牌数
            //if (oldMaxPermits == Double.POSITIVE_INFINITY)
            if (Double.IsPositiveInfinity(oldMaxPermits))
            {//如果oldMaxPermits==整无穷
             // if we don't special-case this, we would get storedPermits == NaN, below
                storedPermits = maxPermits;
            }
            else
            {
                storedPermits =
                    (oldMaxPermits == 0.0)
                        ? 0.0 // initial state
                        : storedPermits * maxPermits / oldMaxPermits;
            }
        }


        protected override long storedPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            return 0L;
        }

        /**
         * 返回恒定的生成令牌的时间间隔
         * */

        protected override double coolDownIntervalMicros()
        {
            return stableIntervalMicros;
        }
    }
}

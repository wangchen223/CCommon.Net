using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.RateLimiter
{
    public class SmoothWarmingUp : SmoothRateLimiter
    {
        private readonly long warmupPeriodMicros;//过度时间
                                                 /**
                                                  * The slope of the line from the stable interval (when permits == 0), to the cold interval
                                                  * (when permits == maxPermits)
                                                  * 从稳定区间（when permits == 0）到冷区间（when permits == maxPermits）的线的斜率
                                                  */
        private double slope;
        /**
        * 生成令牌阈值数
        * https://www.jianshu.com/p/86ef43baba83
        */
        private double thresholdPermits;
        private double coldFactor;

        /**
        * @param warmupPeriod 表示在从冷启动速率过渡到平均速率的时间间隔
        */
        public SmoothWarmingUp(
            SleepingStopwatch stopwatch, long warmupPeriod, TimeUnit timeUnit, double coldFactor) : base(stopwatch)
        {
            this.warmupPeriodMicros = timeUnit.toMicros(warmupPeriod);
            this.coldFactor = coldFactor;
        }

        /**
         * 初始化设置限流器
         * @param stableIntervalMicros 表示生成令牌的时间间隔
         * */

        protected override void doSetRate(double permitsPerSecond, double stableIntervalMicros)
        {
            double oldMaxPermits = maxPermits;
            double coldIntervalMicros = stableIntervalMicros * coldFactor;
            thresholdPermits = 0.5 * warmupPeriodMicros / stableIntervalMicros;//0.5 * 过度时间/令牌生成的时间间隔
            maxPermits =
                thresholdPermits + 2.0 * warmupPeriodMicros / (stableIntervalMicros + coldIntervalMicros);

            //slop可以看作是梯形斜边的斜率，用于计算threshold到maxPermits之间的限流速率
            slope = (coldIntervalMicros - stableIntervalMicros) / (maxPermits - thresholdPermits);
            //if (oldMaxPermits == Double.POSITIVE_INFINITY)
            if (Double.IsPositiveInfinity(oldMaxPermits))
            {
                // if we don't special-case this, we would get storedPermits == NaN, below
                storedPermits = 0.0;
            }
            else
            {
                storedPermits =
                    (oldMaxPermits == 0.0)
                        ? maxPermits // initial state is cold
                        : storedPermits * maxPermits / oldMaxPermits;
            }
        }

        /**
         * 通过storedPermitsToWaitTime计算出获取freshPermits还需要等待的时间
         * @param storedPermits 存储的令牌数
         * @param permitsToTake 本次消耗的令牌数
         * */

        protected override long storedPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            //在阈值以上可用的令牌数目
            double availablePermitsAboveThreshold = storedPermits - thresholdPermits;//存储的令牌数-令牌数的阈值
            long micros = 0;
            // measuring the integral on the right part of the function (the climbing line)
            //如果大于0则说明在过渡段内,
            if (availablePermitsAboveThreshold > 0.0)
            {
                //计算在阈值以上能拿的令牌数目
                double permitsAboveThresholdToTake = Math.Min(availablePermitsAboveThreshold, permitsToTake);
                // TODO(cpovirk): Figure out a good name for this variable.
                double length =
                    permitsToTime(availablePermitsAboveThreshold)
                        + permitsToTime(availablePermitsAboveThreshold - permitsAboveThresholdToTake);
                micros = (long)(permitsAboveThresholdToTake * length / 2.0);
                permitsToTake -= permitsAboveThresholdToTake;
            }
            // measuring the integral on the left part of the function (the horizontal line)
            micros += (long)(stableIntervalMicros * permitsToTake);
            return micros;
        }

        private double permitsToTime(double permits)
        {
            return stableIntervalMicros + permits * slope;//生成令牌时间间隔+速率(默认0.3)*
        }


        protected override double coolDownIntervalMicros()
        {
            return warmupPeriodMicros / maxPermits;//过度时间/最大令牌数
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CCommon.Common.RateLimiter
{
    public abstract class RateLimiter
    {
        protected RateLimiter(SleepingStopwatch stopwatch)
        {
            if (stopwatch == null)
            {
                throw new ArgumentNullException("stopwatch 参数为空");
            }
            this.stopwatch = stopwatch;
        }

        /**
         * Creates a {@code RateLimiter} with the specified stable throughput, given as "permits per
         * second" (commonly referred to as <i>QPS</i>, queries per second).
         *
         * <p>The returned {@code RateLimiter} ensures that on average no more than {@code
         * permitsPerSecond} are issued during any given second, with sustained requests being smoothly
         * spread over each second. When the incoming request rate exceeds {@code permitsPerSecond} the
         * rate limiter will release one permit every {@code (1.0 / permitsPerSecond)} seconds. When the
         * rate limiter is unused, bursts of up to {@code permitsPerSecond} permits will be allowed, with
         * subsequent requests being smoothly limited at the stable rate of {@code permitsPerSecond}.
         *
         * @param permitsPerSecond the rate of the returned {@code RateLimiter}, measured in how many
         *     permits become available per second
         * @throws IllegalArgumentException if {@code permitsPerSecond} is negative or zero
         */
        // TODO(user): "This is equivalent to
        // {@code createWithCapacity(permitsPerSecond, 1, TimeUnit.SECONDS)}".
        public static RateLimiter create(double permitsPerSecond)
        {
            /*
             * The default RateLimiter configuration can save the unused permits of up to one second. This
             * is to avoid unnecessary stalls in situations like this: A RateLimiter of 1qps, and 4 threads,
             * all calling acquire() at these moments:
             *
             * T0 at 0 seconds
             * T1 at 1.05 seconds
             * T2 at 2 seconds
             * T3 at 3 seconds
             *
             * Due to the slight delay of T1, T2 would have to sleep till 2.05 seconds, and T3 would also
             * have to sleep till 3.05 seconds.
             */
            return create(permitsPerSecond, SleepingStopwatch.createFromSystemTimer());
        }

        //@VisibleForTesting
        public static RateLimiter create(double permitsPerSecond, SleepingStopwatch stopwatch)
        {
            RateLimiter rateLimiter = new SmoothBursty(stopwatch, 1.0 /* maxBurstSeconds */);
            rateLimiter.setRate(permitsPerSecond);
            return rateLimiter;
        }

        /**
         * Creates a {@code RateLimiter} with the specified stable throughput, given as "permits per
         * second" (commonly referred to as <i>QPS</i>, queries per second), and a <i>warmup period</i>,
         * during which the {@code RateLimiter} smoothly ramps up its rate, until it reaches its maximum
         * rate at the end of the period (as long as there are enough requests to saturate it). Similarly,
         * if the {@code RateLimiter} is left <i>unused</i> for a duration of {@code warmupPeriod}, it
         * will gradually return to its "cold" state, i.e. it will go through the same warming up process
         * as when it was first created.
         *
         * <p>The returned {@code RateLimiter} is intended for cases where the resource that actually
         * fulfills the requests (e.g., a remote server) needs "warmup" time, rather than being
         * immediately accessed at the stable (maximum) rate.
         *
         * <p>The returned {@code RateLimiter} starts in a "cold" state (i.e. the warmup period will
         * follow), and if it is left unused for long enough, it will return to that state.
         *
         * @param permitsPerSecond the rate of the returned {@code RateLimiter}, measured in how many
         *     permits become available per second
         * @param warmupPeriod the duration of the period where the {@code RateLimiter} ramps up its rate,
         *     before reaching its stable (maximum) rate
         * @param unit the time unit of the warmupPeriod argument
         * @throws IllegalArgumentException if {@code permitsPerSecond} is negative or zero or {@code
         *     warmupPeriod} is negative
         */
        public static RateLimiter create(double permitsPerSecond, long warmupPeriod, TimeUnit unit)
        {
            //checkArgument(warmupPeriod >= 0, "warmupPeriod must not be negative: %s", warmupPeriod);
            return create(
                permitsPerSecond, warmupPeriod, unit, 3.0, SleepingStopwatch.createFromSystemTimer());
        }

        //@VisibleForTesting
        public static RateLimiter create(double permitsPerSecond, long warmupPeriod, TimeUnit unit, double coldFactor, SleepingStopwatch stopwatch)
        {
            RateLimiter rateLimiter = new SmoothWarmingUp(stopwatch, warmupPeriod, unit, coldFactor);
            rateLimiter.setRate(permitsPerSecond);
            return rateLimiter;
        }

        /**
         * The underlying timer; used both to measure elapsed time and sleep as necessary. A separate
         * object to facilitate testing.
         */
        private readonly SleepingStopwatch stopwatch;

        // Can't be initialized in the constructor because mocks don't call the constructor.
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


        /**
         * Updates the stable rate of this {@code RateLimiter}, that is, the {@code permitsPerSecond}
         * argument provided in the factory method that constructed the {@code RateLimiter}. Currently
         * throttled threads will <b>not</b> be awakened as a result of this invocation, thus they do not
         * observe the new rate; only subsequent requests will.
         *
         * <p>Note though that, since each request repays (by waiting, if necessary) the cost of the
         * <i>previous</i> request, this means that the very next request after an invocation to {@code
         * setRate} will not be affected by the new rate; it will pay the cost of the previous request,
         * which is in terms of the previous rate.
         *
         * <p>The behavior of the {@code RateLimiter} is not modified in any other way, e.g. if the {@code
         * RateLimiter} was configured with a warmup period of 20 seconds, it still has a warmup period of
         * 20 seconds after this method invocation.
         *
         * @param permitsPerSecond the new stable rate of this {@code RateLimiter}
         * @throws IllegalArgumentException if {@code permitsPerSecond} is negative or zero
         */
        public void setRate(double permitsPerSecond)
        {
            //checkArgument(permitsPerSecond > 0.0 && !Double.isNaN(permitsPerSecond), "rate must be positive");
            lock (mutex())
            {
                doSetRate(permitsPerSecond, stopwatch.readMicros());
            }
        }

        protected abstract void doSetRate(double permitsPerSecond, long nowMicros);

        /**
         * Returns the stable rate (as {@code permits per seconds}) with which this {@code RateLimiter} is
         * configured with. The initial value of this is the same as the {@code permitsPerSecond} argument
         * passed in the factory method that produced this {@code RateLimiter}, and it is only updated
         * after invocations to {@linkplain #setRate}.
         */
        public double getRate()
        {
            lock (mutex())
            {
                return doGetRate();
            }
        }

        protected abstract double doGetRate();

        /**
         * Acquires a single permit from this {@code RateLimiter}, blocking until the request can be
         * granted. Tells the amount of time slept, if any.
         *
         * <p>This method is equivalent to {@code acquire(1)}.
         *
         * @return time spent sleeping to enforce rate, in seconds; 0.0 if not rate-limited
         * @since 16.0 (present in 13.0 with {@code void} return type})
         */
        //@CanIgnoreReturnValue
        public double acquire()
        {
            return acquire(1);
        }

        /**
         * Acquires the given number of permits from this {@code RateLimiter}, blocking until the request
         * can be granted. Tells the amount of time slept, if any.
         *
         * @param permits the number of permits to acquire
         * @return time spent sleeping to enforce rate, in seconds; 0.0 if not rate-limited
         * @throws IllegalArgumentException if the requested number of permits is negative or zero
         * @since 16.0 (present in 13.0 with {@code void} return type})
         */
        //@CanIgnoreReturnValue
        public double acquire(int permits)
        {
            long microsToWait = reserve(permits);
            Console.WriteLine("microsToWait:" + microsToWait);
            stopwatch.sleepMicrosUninterruptibly(microsToWait);
            return 1.0 * microsToWait / TimeUnit.Seconds.toMicros(1L);
        }

        /**
         * Reserves the given number of permits from this {@code RateLimiter} for future use, returning
         * the number of microseconds until the reservation can be consumed.
         *
         * @return time in microseconds to wait until the resource can be acquired, never negative
         */
        long reserve(int permits)
        {
            checkPermits(permits);
            lock (mutex())
            {
                   return reserveAndGetWaitLength(permits, stopwatch.readMicros());
            }
        }

        /**
         * Acquires a permit from this {@code RateLimiter} if it can be obtained without exceeding the
         * specified {@code timeout}, or returns {@code false} immediately (without waiting) if the permit
         * would not have been granted before the timeout expired.
         *
         * <p>This method is equivalent to {@code tryAcquire(1, timeout, unit)}.
         *
         * @param timeout the maximum time to wait for the permit. Negative values are treated as zero.
         * @param unit the time unit of the timeout argument
         * @return {@code true} if the permit was acquired, {@code false} otherwise
         * @throws IllegalArgumentException if the requested number of permits is negative or zero
         */
        public bool tryAcquire(long timeout, TimeUnit unit)
        {
            return tryAcquire(1, timeout, unit);
        }

        /**
         * Acquires permits from this {@link RateLimiter} if it can be acquired immediately without delay.
         *
         * <p>This method is equivalent to {@code tryAcquire(permits, 0, anyUnit)}.
         *
         * @param permits the number of permits to acquire
         * @return {@code true} if the permits were acquired, {@code false} otherwise
         * @throws IllegalArgumentException if the requested number of permits is negative or zero
         * @since 14.0
         */
        public bool tryAcquire(int permits)
        {
            return tryAcquire(permits, 0, TimeUnit.Microseconds);
        }

        /**
         * Acquires a permit from this {@link RateLimiter} if it can be acquired immediately without
         * delay.
         *
         * <p>This method is equivalent to {@code tryAcquire(1)}.
         *
         * @return {@code true} if the permit was acquired, {@code false} otherwise
         * @since 14.0
         */
        public bool tryAcquire()
        {
            return tryAcquire(1, 0, TimeUnit.Microseconds);
        }

        /**
         * Acquires the given number of permits from this {@code RateLimiter} if it can be obtained
         * without exceeding the specified {@code timeout}, or returns {@code false} immediately (without
         * waiting) if the permits would not have been granted before the timeout expired.
         *
         * @param permits the number of permits to acquire
         * @param timeout the maximum time to wait for the permits. Negative values are treated as zero.
         * @param unit the time unit of the timeout argument
         * @return {@code true} if the permits were acquired, {@code false} otherwise
         * @throws IllegalArgumentException if the requested number of permits is negative or zero
         */
        public bool tryAcquire(int permits, long timeout, TimeUnit unit)
        {
            long timeoutMicros = Math.Max(unit.toMicros(timeout), 0);
            checkPermits(permits);
            long microsToWait;
            lock (mutex())
            {
                long nowMicros = stopwatch.readMicros();
                if (!canAcquire(nowMicros, timeoutMicros))
                {
                    return false;
                }
                else
                {
                    microsToWait = reserveAndGetWaitLength(permits, nowMicros);
                }
            }
            stopwatch.sleepMicrosUninterruptibly(microsToWait);
            return true;
        }

        private bool canAcquire(long nowMicros, long timeoutMicros)
        {
            return queryEarliestAvailable(nowMicros) - timeoutMicros <= nowMicros;
        }


        /// <summary>
        /// 获取令牌，并返回需要等待的时间，单位微秒
        /// </summary>
        /// <param name="permits"></param>
        /// <param name="nowMicros"></param>
        /// <returns></returns>
        /// <remarks>
        ///        * Reserves next ticket and returns the wait time that the caller must wait for.
        ///        *
        ///        * @return the required wait time, never negative
        /// </remarks>
        long reserveAndGetWaitLength(int permits, long nowMicros)
        {
            long momentAvailable = reserveEarliestAvailable(permits, nowMicros);
            return Math.Max(momentAvailable - nowMicros, 0);
        }



        /**
         * Returns the earliest time that permits are available (with one caveat).
         *
         * @return the time that permits are available, or, if permits are available immediately, an
         *     arbitrary past or present time
         */
        protected abstract long queryEarliestAvailable(long nowMicros);

        /// <summary>
        ///  获取最近的一个可用令牌的时间
        /// 从`reserveEarliestAvailable`可以看出RateLimiter的预消费原理，以及获取令牌的等待时间时间原理（可以解释示例结果），再获取令牌不足时，并没有等待到令牌全部生成，而是更新了下次获取令牌时的nextFreeTicketMicros，从而影响的是下次获取令牌的等待时间。
        /// </summary>
        /// <param name="permits"></param>
        /// <param name="nowMicros"></param>
        /// <returns></returns>
        /// <remarks>
        /// Reserves the requested number of permits and returns the time that those permits can be used
        /// (with one caveat).
        ///
        /// @return the time that the permits may be used, or, if the permits may be used immediately, an
        ///     arbitrary past or present time
        /// </remarks>
        protected abstract long reserveEarliestAvailable(int permits, long nowMicros);


        public override string ToString()
        {
            return base.ToString();
            //return String.Format(Locale.ROOT, "RateLimiter[stableRate=%3.1fqps]", getRate());
        }

        private static void checkPermits(int permits)
        {
            //checkArgument(permits > 0, "Requested permits (%s) must be positive", permits);
        }


        public static void Test()
        {
            //平滑令牌限流
            var rate = RateLimiter.create(1);
            for (int i = 0; i < 1000; i++)
            {
                var time = rate.acquire(2);
                Console.WriteLine("i:" + i++ + "_Date:" + DateTime.Now.ToString("HH:mm:ss,ffffff") + "_RunTime:" + time);
                Console.WriteLine("-------------------------------------------------------");
            }

            //滑动令牌限流
            RateLimiter limiter = RateLimiter.create(10, 10, TimeUnit.Seconds);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffff"));
            for (int i = 0; i < 10000; i++)
            {
                //获取一个令牌
                Console.WriteLine(i + ":" + limiter.acquire(1) + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffff"));
            }
        }
    }
}
﻿using System;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 日期时间帮助类
    /// </summary>
    internal static class DateTimeHelper
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        public static DateTimeOffset FloorToSeconds(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.AddTicks(-GetExtraTicks(dateTimeOffset.Ticks));
        }

        public static bool IsRound(DateTimeOffset dateTimeOffset)
        {
            return GetExtraTicks(dateTimeOffset.Ticks) == 0;
        }

        private static long GetExtraTicks(long ticks)
        {
            return ticks % OneSecond.Ticks;
        }
    }
}

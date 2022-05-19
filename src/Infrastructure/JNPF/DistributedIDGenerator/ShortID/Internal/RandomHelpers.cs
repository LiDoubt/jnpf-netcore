using System;

namespace JNPF.DistributedIDGenerator
{
    /// <summary>
    /// 随机数帮助类
    /// </summary>
    internal static class RandomHelpers
    {
        /// <summary>
        /// 随机数对象
        /// </summary>
        private static readonly Random Random = new();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object ThreadLock = new();

        /// <summary>
        /// 生成线程安全的范围内随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GenerateNumberInRange(int min, int max)
        {
            lock (ThreadLock)
            {
                return Random.Next(min, max);
            }
        }
    }
}

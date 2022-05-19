using JNPF.Dependency;
using System;

namespace JNPF.Common.Collections
{
    /// <summary>
    /// 数组扩展方法
    /// </summary>
    [SuppressSniffer]
    public static class ArrayExtensions
    {
        /// <summary>
        /// 复制一份二维数组的副本
        /// </summary>
        public static byte[,] Copy(this byte[,] bytes)
        {
            int width = bytes.GetLength(0), height = bytes.GetLength(1);
            byte[,] newBytes = new byte[width, height];
            Array.Copy(bytes, newBytes, bytes.Length);
            return newBytes;
        }
    }
}

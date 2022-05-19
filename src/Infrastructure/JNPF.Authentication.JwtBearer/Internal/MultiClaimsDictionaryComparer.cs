using System.Collections.Generic;

namespace JNPF.Authorization
{
    /// <summary>
    /// 解决 Claims 身份重复键问题
    /// </summary>
    internal sealed class MultiClaimsDictionaryComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// 设置字符串永不相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(string x, string y)
        {
            return x != y;
        }

        /// <summary>
        /// 返回字符串 hashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}

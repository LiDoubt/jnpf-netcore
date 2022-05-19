using JNPF.Dependency;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace JNPF.DataEncryption
{
    /// <summary>
    /// MD5 加密
    /// </summary>
    [SuppressSniffer]
    public static unsafe class MD5Encryption
    {
        /// <summary>
        /// 字符串 MD5 比较
        /// </summary>
        /// <param name="text">加密文本</param>
        /// <param name="hash">MD5 字符串</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns>bool</returns>
        public static bool Compare(string text, string hash, bool uppercase = false)
        {
            var hashOfInput = Encrypt(text, uppercase);
            return hash.Equals(hashOfInput, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="text">加密文本</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns></returns>
        public static string Encrypt(string text, bool uppercase = false)
        {
            using var md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            var hash = stringBuilder.ToString();
            return !uppercase ? hash : hash.ToUpper();
        }
    }
}
﻿using JNPF.Dependency;

namespace JNPF.DataEncryption.Extensions
{
    /// <summary>
    /// DataEncryption 字符串加密拓展
    /// </summary>
    [SuppressSniffer]
    public static class StringEncryptionExtensions
    {
        /// <summary>
        /// 字符串的 MD5
        /// </summary>
        /// <param name="text"></param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns>string</returns>
        public static string ToMD5Encrypt(this string text, bool uppercase = false)
        {
            return MD5Encryption.Encrypt(text, uppercase);
        }

        /// <summary>
        /// 字符串的 MD5
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hash"></param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns>string</returns>
        public static bool ToMD5Compare(this string text, string hash, bool uppercase = false)
        {
            return MD5Encryption.Compare(text, hash, uppercase);
        }

        /// <summary>
        /// 字符串 AES 加密
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <param name="skey"></param>
        /// <returns>string</returns>
        public static string ToAESEncrypt(this string text, string skey)
        {
            return AESEncryption.Encrypt(text, skey);
        }

        /// <summary>
        /// 字符串 AES 解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="skey"></param>
        /// <returns>string</returns>
        public static string ToAESDecrypt(this string text, string skey)
        {
            return AESEncryption.Decrypt(text, skey);
        }

        /// <summary>
        /// 字符串 DESC 加密
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <param name="skey">密钥</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns>string</returns>
        public static string ToDESCEncrypt(this string text, string skey, bool uppercase = false)
        {
            return DESCEncryption.Encrypt(text, skey, uppercase);
        }

        /// <summary>
        /// 字符串 DESC 解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="skey">密钥</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <returns>string</returns>
        public static string ToDESCDecrypt(this string text, string skey, bool uppercase = false)
        {
            return DESCEncryption.Decrypt(text, skey, uppercase);
        }

        /// <summary>
        /// 字符串 RSA 加密
        /// </summary>
        /// <param name="text">需要加密的文本</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string ToRSAEncrpyt(this string text, string publicKey)
        {
            return RSAEncryption.Encrypt(text, publicKey);
        }

        /// <summary>
        /// 字符串 RSA 解密
        /// </summary>
        /// <param name="text">需要解密的文本</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string ToRSADecrypt(this string text, string privateKey)
        {
            return RSAEncryption.Decrypt(text, privateKey);
        }
    }
}

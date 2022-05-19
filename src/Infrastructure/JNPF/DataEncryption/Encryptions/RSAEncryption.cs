﻿using JNPF.Dependency;
using System;
using System.Security.Cryptography;
using System.Text;

namespace JNPF.DataEncryption
{
    /// <summary>
    /// RSA 加密
    /// </summary>
    [SuppressSniffer]
    public static class RSAEncryption
    {
        /// <summary>
        /// 生成 RSA 秘钥
        /// </summary>
        /// <param name="keySize">大小必须为 2048 到 16384 之间，且必须能被 8 整除</param>
        /// <returns></returns>
        public static (string publicKey, string privateKey) GenerateSecretKey(int keySize = 2048)
        {
            CheckRSAKeySize(keySize);

            using var rsa = new RSACryptoServiceProvider(keySize);
            return (rsa.ToXmlString(false), rsa.ToXmlString(true));
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">明文内容</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static string Encrypt(string text, string publicKey, int keySize = 2048)
        {
            CheckRSAKeySize(keySize);

            using var rsa = new RSACryptoServiceProvider(keySize);
            rsa.FromXmlString(publicKey);

            var encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(text), false);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">密文内容</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static string Decrypt(string text, string privateKey, int keySize = 2048)
        {
            CheckRSAKeySize(keySize);

            using var rsa = new RSACryptoServiceProvider(keySize);
            rsa.FromXmlString(privateKey);

            var decryptedData = rsa.Decrypt(Convert.FromBase64String(text), false);
            return Encoding.Default.GetString(decryptedData);
        }

        /// <summary>
        /// 检查 RSA 长度
        /// </summary>
        /// <param name="keySize"></param>
        private static void CheckRSAKeySize(int keySize)
        {
            if (keySize < 2048 || keySize > 16384 || keySize % 8 != 0)
                throw new ArgumentException("The keySize must be between 2048 and 16384 in size and must be divisible by 8.", nameof(keySize));
        }
    }
}

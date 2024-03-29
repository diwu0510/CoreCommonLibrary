﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace HZC.Utils
{
    public static class Sha1EncryptUtil
    {
        /// <summary>
        /// 加密字符串，默认编码Encoding.UTF8
        /// </summary>
        /// <param name="content">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string content)
        {
            return Encrypt(content, Encoding.UTF8);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="content">要加密的字符串</param>
        /// <param name="encode">字符串编码</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                var bytesIn = encode.GetBytes(content);
                var bytesOut = sha1.ComputeHash(bytesIn);
                sha1.Dispose();

                var result = BitConverter.ToString(bytesOut);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密失败：" + ex.Message);
            }
        }
    }
}

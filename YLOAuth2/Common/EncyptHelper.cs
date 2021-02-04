using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace YLOAuth2.Common
{
    public static class CryptHelper
    {


        /// <summary>
        /// MD5加密(16进制)
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="mode">加密模式（只有16和32这两种模式）</param>
        /// <param name="salt">加盐</param>
        /// <returns>返回加密后的内容</returns>
        public static string EncryptByMD5(string content, int mode = 32, string salt = "")
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var cyStr = content;
            if (!string.IsNullOrEmpty(salt))
            {
                cyStr = content + salt;
            }
            var byteArr = Encoding.GetEncoding("UTF-8").GetBytes(cyStr);
            var hashBytes = md5.ComputeHash(byteArr);
            StringBuilder sb = new StringBuilder();
            foreach (byte i in hashBytes)
            {
                sb.Append(i.ToString("x2"));
            }
            var res = string.Empty;
            if (mode == 16)
            {
                res = sb.ToString().Substring(8, 16);
            }
            else if (mode == 32)
            {
                res = sb.ToString();//默认情况
            }
            return res;
        }

        /// <summary>
        /// MD5加密(Base64)
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="mode">加密模式（只有16和32这两种模式）</param>
        /// <param name="salt">加盐</param>
        /// <returns>返回加密后的内容</returns>
        public static string EncryptByMD5_Base64(string content, int mode = 32, string salt = "")
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var cyStr = content;
            if (!string.IsNullOrEmpty(salt))
            {
                cyStr = content + salt;
            }
            var byteArr = Encoding.GetEncoding("UTF-8").GetBytes(cyStr);
            var hashBytes = md5.ComputeHash(byteArr);
            StringBuilder sb = new StringBuilder();
            foreach (byte i in hashBytes)
            {
                sb.Append(i.ToString("x2"));
            }
            var res = string.Empty;
            if (mode == 16)
            {
                res = sb.ToString().Substring(8, 16);
            }
            else if (mode == 32)
            {
                res = sb.ToString();//默认情况
            }
            var base64Str = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(res));
            return base64Str;
        }

        #region 对称加解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="key">密钥</param>
        /// <returns>返回加密后的内容</returns>
        public static string EncryptByDES(string content, string key)
        {
            /*
             *DESCryptoServiceProvider 中的密钥是8位；
             *RijndaelManaged 中的密钥是32位。
             * 否则会报错
             */
            if (key.Length != 8)
            {
                throw new MyException("密钥位数只能是8位！");
            }
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(content);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.GetEncoding("UTF-8").GetBytes(key);
            des.IV = Encoding.GetEncoding("UTF-8").GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write); //将数据流连接到加密转换的流
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            /*得到字节流后，后续可以转成base64位字符串或者16进制的字符串
             * 下面是转成16进制字符串（2位数不足的补0）
            */
            StringBuilder res = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                res.AppendFormat("{0:X2}", b);
                /*
                 * 转化为16进制字符串。
                 * 大写X：ToString("X2")即转化为大写的16进制。
                 * 小写x：ToString("x2")即转化为小写的16进制。
                 * 2表示输出两位，不足2位的前面补0,如 0x0A 如果没有2,就只会输出0xA
                 */
            }
            res.ToString();
            return res.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="content">待解密的内容</param>
        /// <param name="key">密钥</param>
        /// <returns>返回解密后的内容</returns>
        public static string DecryptByDES(string content, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //加密的时候遍历了密文字节流数组，字节流数组中的一个元素对应生成密文字符串的两位，这个用密文字符串长度除以2得到原来加密时得到的字节流数组长度
            byte[] inputByteArray = new byte[content.Length / 2];
            for (int x = 0; x < content.Length / 2; x++)
            {
                int i = (Convert.ToInt32(content.Substring(x * 2, 2), 16)); //将16进制的数据转成10进制
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.GetEncoding("UTF-8").GetBytes(key);
            des.IV = Encoding.GetEncoding("UTF-8").GetBytes(key);
            MemoryStream ms = new MemoryStream();
            try
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                //当提供的密钥与加密时的密钥不一致会导致报错
                cs.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                throw new Exception("密钥错误！");
            }



            StringBuilder ret = new StringBuilder();
            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string EncryptByAES(string content, string key)
        {
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(content);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = ASCIIEncoding.GetEncoding("UTF-8").GetBytes(key); //在AES的情况下，密钥是32位
            aes.IV = Encoding.GetEncoding("UTF-8").GetBytes(key.Substring(0, 16)); //在AES的情况下，该大小为16字节 
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write); //将数据流连接到加密转换的流
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            /*得到字节流后，后续可以转成base64位字符串或者16进制的字符串
             * 当前采用：转成base64位字符串
            */
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// AES解密 
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DecryptByAES(string content, string key)
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(content);
            aes.Key = Encoding.GetEncoding("UTF-8").GetBytes(key);//在AES的情况下，密钥是32位
            aes.IV = Encoding.GetEncoding("UTF-8").GetBytes(key.Substring(0,16)); //在AES的情况下，该大小为16字节 
            MemoryStream ms = new MemoryStream();
            try
            {
                CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                //当提供的密钥与加密时的密钥不一致会导致报错
                cs.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                throw new Exception("密钥错误！");
            }
            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
        }

        #endregion

        #region 非对称加解密

        /*
         * RSA非对称加密，支持公钥加密私钥解密，或者私钥加密公钥解密，但是C#却只实现了公钥加密私钥解密，
         * 如果我们需要实现私钥加密公钥解密，就需要使用到第三方插件，“BouncyCastle.Crypto”
         */

        /// <summary>
        /// 生成一对公钥和私钥
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetKeyPair1()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }
        /// <summary>
        /// 生成一对公钥和私钥
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetKeyPair2()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }


        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <param name="pulicKey">公钥</param>
        /// <returns></returns>
        public static string EncryptByRSA(string content, string pulicKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //对应获取密钥对的模式选择用哪一种
                rsa.FromXmlString(pulicKey);
               // rsa.ImportCspBlob(Convert.FromBase64String(pulicKey));      
                var contentBytes = Encoding.GetEncoding("UTF-8").GetBytes(content);
                var cryptBytes = rsa.Encrypt(contentBytes, false);
                var res = Convert.ToBase64String(cryptBytes);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="content">待解密内容</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string DecryptByRSA(string content, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //对应获取密钥对的模式选择用哪一种
                rsa.FromXmlString(privateKey);
                // rsa.ImportCspBlob(Convert.FromBase64String(pulicKey));
                var contentBytes = Convert.FromBase64String(content);
                var cryptBytes = rsa.Decrypt(contentBytes, false);
                var res = Encoding.GetEncoding("UTF-8").GetString(cryptBytes);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
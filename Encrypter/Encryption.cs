using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MrHuo.Controls.Encrypter
{
    /// <summary>
    /// 对称加密算法类
    /// </summary>
    public class Encryption
    {
        private SymmetricAlgorithm mobjCryptoService;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Encryption()
        {
            mobjCryptoService = new RijndaelManaged();
        }

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey(string key)
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();
            int KeyLength = mobjCryptoService.Key.Length;

            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return Encoding.UTF8.GetBytes(sTemp);
        }
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = Convert.ToBase64String(Encoding.UTF8.GetBytes("ghUb#erhj*Ghg7!rNIfb&95GUY86Gfg4ui%$hjkE457HBh(u%g6HJ($jhWk7&!hg"));
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return Encoding.UTF8.GetBytes(sTemp);
        }
        /// <summary>
        /// 用Key加密执行字符串
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <param name="key">私钥</param>
        /// <returns>经过加密的串</returns>
        public string Encrypto(string Source, string key)
        {
            try
            {
                byte[] bytIn = Encoding.UTF8.GetBytes(Source);
                MemoryStream ms = new MemoryStream();
                mobjCryptoService.Key = GetLegalKey(key);
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <param name="key">私钥</param>
        /// <returns>经过解密的串</returns>
        public string Decrypto(string Source, string key)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mobjCryptoService.Key = GetLegalKey(key);
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

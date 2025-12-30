/// <returns>
/// timeRFC3339
/// </returns>
// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Tea;
using Tea.Utils;


namespace ToolsetLink.DarabonbaBaseCSharp
{
    public class Client 
    {

        public static string TimeRFC3339()
        {
            // Go的time.RFC3339格式为"2006-01-02T15:04:05Z07:00"，C#中的"O"格式与此兼容
            return DateTime.UtcNow.ToString("O");
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>生成16位随机Nonce</para>
        /// </description>
        /// 
        /// <returns>
        /// generateNonce
        /// </returns>
        public static string GenerateNonce()
        {
            // 与Go版本保持一致：生成8字节随机数，然后转换为16位十六进制字符串
            byte[] bytes = new byte[8];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>生成签名</para>
        /// </description>
        /// 
        /// <returns>
        /// generateSignature
        /// </returns>
        public static string GenerateSignature(string body, string nonce, string secretKey, string timestamp, string uri)
        {
            // 与Go版本保持一致的签名生成逻辑
            System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(body))
            {
                parts.Add("body=" + body);
            }

            parts.AddRange(new string[]
            {
                "nonce=" + nonce,
                "secretKey=" + secretKey,
                "timestamp=" + timestamp,
                "url=" + uri
            });

            string signStr = string.Join("&", parts);
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signStr));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}

using System;
using System.Text.RegularExpressions;
using Xunit;
using ToolsetLink.DarabonbaBaseCSharp;

namespace ToolsetLink.DarabonbaBaseCSharpTest
{
    public class ClientTests
    {
        [Fact]
        public void TestTimeRFC3339()
        {
            // Act
            string result = Client.TimeRFC3339();
            
            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result));
            
            // 验证返回的时间格式是否符合RFC3339标准
            Assert.True(DateTime.TryParse(result, out DateTime parsedDateTime));
            
            // 验证时间格式包含时区信息(Z表示UTC)
            Assert.EndsWith("Z", result);
            
            // 验证时间格式符合秒级ISO 8601标准
            Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", result);
        }

        [Fact]
        public void TestGenerateNonce()
        {
            // Act
            string result = Client.GenerateNonce();
            
            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result));
            
            // 验证生成的nonce长度为16位十六进制字符串(8字节)
            Assert.Equal(16, result.Length);
            
            // 验证生成的nonce只包含小写十六进制字符
            Assert.True(Regex.IsMatch(result, "^[0-9a-f]+$"));
            
            // 验证多次调用生成不同的nonce
            string nonce1 = Client.GenerateNonce();
            string nonce2 = Client.GenerateNonce();
            Assert.NotEqual(nonce1, nonce2);
        }

        [Fact]
        public void TestGenerateSignature_WithBody()
        {
            string body = "{\"winKey\":\"npJi367lttpwmD1goZ1yOQ\",\"arch\":\"x64\",\"versionCode\":1,\"appointVersionCode\":0,\"devModelKey\":\"\",\"devKey\":\"\"}";
            string nonce = "b78a9459e9e1d340";
            string secretKey = "PEbdHFGC0uO_Pch7XWBQTMsFRxKPQAM2565eP8LJ3gc";
            string timestamp = "2026-01-05T03:28:31Z";
            string uri = "/v1/win/upgrade";
            
            string result = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            Console.WriteLine($"Generated Signature: {result}");
            
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Equal(32, result.Length);
            Assert.True(Regex.IsMatch(result, "^[0-9a-f]+$"));
        }

       
    }
}
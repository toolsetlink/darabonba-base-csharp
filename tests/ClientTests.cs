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
            
            // 验证时间格式符合ISO 8601标准
            Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+Z$", result);
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
            // Arrange
            string body = "testBody";
            string nonce = "testNonce123456";
            string secretKey = "testSecretKey";
            string timestamp = "1234567890";
            string uri = "/test/uri";
            
            // Act
            string result = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            
            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result));
            
            // 验证签名长度符合MD5哈希结果(32位)
            Assert.Equal(32, result.Length);
            
            // 验证相同参数生成相同签名
            string result2 = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            Assert.Equal(result, result2);
            
            // 验证生成的签名只包含小写十六进制字符
            Assert.True(Regex.IsMatch(result, "^[0-9a-f]+$"));
        }

        [Fact]
        public void TestGenerateSignature_WithoutBody()
        {
            // Arrange
            string body = "";
            string nonce = "testNonce123456";
            string secretKey = "testSecretKey";
            string timestamp = "1234567890";
            string uri = "/test/uri";
            
            // Act
            string result = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            
            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result));
            
            // 验证签名长度符合MD5哈希结果(32位)
            Assert.Equal(32, result.Length);
            
            // 验证生成的签名只包含小写十六进制字符
            Assert.True(Regex.IsMatch(result, "^[0-9a-f]+$"));
        }

        [Fact]
        public void TestGenerateSignature_DifferentParameters()
        {
            // Arrange - 基础参数
            string body = "testBody";
            string nonce = "testNonce123456";
            string secretKey = "testSecretKey";
            string timestamp = "1234567890";
            string uri = "/test/uri";
            
            // Act
            string baseSignature = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            
            // Assert - 不同参数应生成不同签名
            string differentBodySignature = Client.GenerateSignature("differentBody", nonce, secretKey, timestamp, uri);
            Assert.NotEqual(baseSignature, differentBodySignature);
            
            string differentNonceSignature = Client.GenerateSignature(body, "differentNonce", secretKey, timestamp, uri);
            Assert.NotEqual(baseSignature, differentNonceSignature);
            
            string differentSecretKeySignature = Client.GenerateSignature(body, nonce, "differentSecretKey", timestamp, uri);
            Assert.NotEqual(baseSignature, differentSecretKeySignature);
            
            string differentTimestampSignature = Client.GenerateSignature(body, nonce, secretKey, "9876543210", uri);
            Assert.NotEqual(baseSignature, differentTimestampSignature);
            
            string differentUriSignature = Client.GenerateSignature(body, nonce, secretKey, timestamp, "/different/uri");
            Assert.NotEqual(baseSignature, differentUriSignature);
        }
    }
}
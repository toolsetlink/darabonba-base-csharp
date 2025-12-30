using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsetLink.DarabonbaBaseCSharp;

namespace ToolsetLink.DarabonbaBaseCSharpTest
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void TestTimeRFC3339()
        {
            // Act
            string result = Client.TimeRFC3339();
            
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            
            // 验证返回的时间格式是否符合RFC3339标准
            Assert.IsTrue(DateTime.TryParse(result, out DateTime parsedDateTime));
            
            // 验证时间格式包含时区信息(Z表示UTC)
            Assert.IsTrue(result.EndsWith("Z"));
            
            // 验证解析的时间与当前时间相近(±1分钟)
            DateTime now = DateTime.UtcNow;
            Assert.IsTrue(parsedDateTime >= now.AddMinutes(-1) && parsedDateTime <= now.AddMinutes(1));
        }

        [TestMethod]
        public void TestGenerateNonce()
        {
            // Act
            string result = Client.GenerateNonce();
            
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            
            // 验证生成的nonce长度为16位十六进制字符串(8字节)
            Assert.AreEqual(16, result.Length);
            
            // 验证生成的nonce只包含小写十六进制字符
            Assert.IsTrue(Regex.IsMatch(result, "^[0-9a-f]+$"));
            
            // 验证多次调用生成不同的nonce
            string nonce1 = Client.GenerateNonce();
            string nonce2 = Client.GenerateNonce();
            Assert.AreNotEqual(nonce1, nonce2);
        }

        [TestMethod]
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
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            
            // 验证签名长度符合MD5哈希结果(32位)
            Assert.AreEqual(32, result.Length);
            
            // 验证相同参数生成相同签名
            string result2 = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
            Assert.AreEqual(result, result2);
            
            // 验证生成的签名只包含小写十六进制字符
            Assert.IsTrue(Regex.IsMatch(result, "^[0-9a-f]+$"));
        }

        [TestMethod]
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
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            
            // 验证签名长度符合MD5哈希结果(32位)
            Assert.AreEqual(32, result.Length);
            
            // 验证生成的签名只包含小写十六进制字符
            Assert.IsTrue(Regex.IsMatch(result, "^[0-9a-f]+$"));
        }

        [TestMethod]
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
            Assert.AreNotEqual(baseSignature, differentBodySignature);
            
            string differentNonceSignature = Client.GenerateSignature(body, "differentNonce", secretKey, timestamp, uri);
            Assert.AreNotEqual(baseSignature, differentNonceSignature);
            
            string differentSecretKeySignature = Client.GenerateSignature(body, nonce, "differentSecretKey", timestamp, uri);
            Assert.AreNotEqual(baseSignature, differentSecretKeySignature);
            
            string differentTimestampSignature = Client.GenerateSignature(body, nonce, secretKey, "9876543210", uri);
            Assert.AreNotEqual(baseSignature, differentTimestampSignature);
            
            string differentUriSignature = Client.GenerateSignature(body, nonce, secretKey, timestamp, "/different/uri");
            Assert.AreNotEqual(baseSignature, differentUriSignature);
        }
    }
}
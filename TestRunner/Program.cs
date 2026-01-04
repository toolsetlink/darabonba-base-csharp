using System;
using System.Text.RegularExpressions;
using ToolsetLink.DarabonbaBaseCSharp;

namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始测试 Darabonba Base CSharp 客户端方法...");
            Console.WriteLine("==========================================");
            
            int passedTests = 0;
            int totalTests = 0;
            
            // 测试 TimeRFC3339 方法
            Console.WriteLine("\n1. 测试 TimeRFC3339 方法:");
            totalTests++;
            try
            {
                string result = Client.TimeRFC3339();
                Console.WriteLine($"   结果: {result}");
                
                // 验证返回的时间格式是否符合RFC3339标准
                if (DateTime.TryParse(result, out DateTime parsedDateTime))
                {
                    Console.WriteLine("   ✅ 时间格式有效");
                    
                    // 验证时间格式包含时区信息(Z表示UTC)
                    if (result.EndsWith("Z"))
                    {
                        Console.WriteLine("   ✅ 包含UTC时区标记");
                        
                        // 验证解析的时间与当前时间相近(±1分钟)
                        DateTime now = DateTime.UtcNow;
                        if (parsedDateTime >= now.AddMinutes(-1) && parsedDateTime <= now.AddMinutes(1))
                        {
                            Console.WriteLine("   ✅ 时间与当前时间相近");
                            passedTests++;
                        }
                        else
                        {
                            Console.WriteLine("   ❌ 时间与当前时间相差过大");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 缺少UTC时区标记");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ 时间格式无效");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 测试失败: {ex.Message}");
            }
            
            // 测试 GenerateNonce 方法
            Console.WriteLine("\n2. 测试 GenerateNonce 方法:");
            totalTests++;
            try
            {
                string result = Client.GenerateNonce();
                Console.WriteLine($"   结果: {result}");
                
                // 验证生成的nonce长度为16位十六进制字符串(8字节)
                if (result.Length == 16)
                {
                    Console.WriteLine("   ✅ 长度为16位");
                    
                    // 验证生成的nonce只包含小写十六进制字符
                    if (Regex.IsMatch(result, "^[0-9a-f]+$"))
                    {
                        Console.WriteLine("   ✅ 只包含小写十六进制字符");
                        
                        // 验证多次调用生成不同的nonce
                        string nonce1 = Client.GenerateNonce();
                        string nonce2 = Client.GenerateNonce();
                        if (nonce1 != nonce2)
                        {
                            Console.WriteLine("   ✅ 多次调用生成不同的nonce");
                            passedTests++;
                        }
                        else
                        {
                            Console.WriteLine("   ❌ 多次调用生成相同的nonce");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 包含非小写十六进制字符");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ 长度不是16位");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 测试失败: {ex.Message}");
            }
            
            // 测试 GenerateSignature 方法
            Console.WriteLine("\n3. 测试 GenerateSignature 方法:");
            totalTests++;
            try
            {
                // 测试用例1: 带有body的签名
                string body = "testBody";
                string nonce = "testNonce123456";
                string secretKey = "testSecretKey";
                string timestamp = "1234567890";
                string uri = "/test/uri";
                
                string result1 = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
                Console.WriteLine($"   用例1 (带body) 结果: {result1}");
                
                // 测试用例2: 不带body的签名
                string result2 = Client.GenerateSignature("", nonce, secretKey, timestamp, uri);
                Console.WriteLine($"   用例2 (无body) 结果: {result2}");
                
                // 测试用例3: 相同参数生成相同签名
                string result3 = Client.GenerateSignature(body, nonce, secretKey, timestamp, uri);
                Console.WriteLine($"   用例3 (相同参数) 结果: {result3}");
                
                // 验证签名长度符合MD5哈希结果(32位)
                if (result1.Length == 32 && result2.Length == 32 && result3.Length == 32)
                {
                    Console.WriteLine("   ✅ 签名长度符合MD5标准");
                    
                    // 验证生成的签名只包含小写十六进制字符
                    if (Regex.IsMatch(result1, "^[0-9a-f]+$") && Regex.IsMatch(result2, "^[0-9a-f]+") && Regex.IsMatch(result3, "^[0-9a-f]+") )
                    {
                        Console.WriteLine("   ✅ 只包含小写十六进制字符");
                        
                        // 验证相同参数生成相同签名
                        if (result1 == result3)
                        {
                            Console.WriteLine("   ✅ 相同参数生成相同签名");
                            passedTests++;
                        }
                        else
                        {
                            Console.WriteLine("   ❌ 相同参数生成不同签名");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 包含非小写十六进制字符");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ 签名长度不符合MD5标准");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 测试失败: {ex.Message}");
            }
            
            // 测试 GenerateSignature 方法的不同参数
            Console.WriteLine("\n4. 测试 GenerateSignature 方法的不同参数:");
            totalTests++;
            try
            {
                // 基础参数
                string baseBody = "testBody";
                string baseNonce = "testNonce123456";
                string baseSecretKey = "testSecretKey";
                string baseTimestamp = "1234567890";
                string baseUri = "/test/uri";
                
                string baseSignature = Client.GenerateSignature(baseBody, baseNonce, baseSecretKey, baseTimestamp, baseUri);
                Console.WriteLine($"   基础签名: {baseSignature}");
                
                // 测试不同body
                string differentBodySignature = Client.GenerateSignature("differentBody", baseNonce, baseSecretKey, baseTimestamp, baseUri);
                Console.WriteLine($"   不同body: {differentBodySignature}");
                
                // 测试不同nonce
                string differentNonceSignature = Client.GenerateSignature(baseBody, "differentNonce", baseSecretKey, baseTimestamp, baseUri);
                Console.WriteLine($"   不同nonce: {differentNonceSignature}");
                
                // 测试不同secretKey
                string differentSecretKeySignature = Client.GenerateSignature(baseBody, baseNonce, "differentSecretKey", baseTimestamp, baseUri);
                Console.WriteLine($"   不同secretKey: {differentSecretKeySignature}");
                
                // 测试不同timestamp
                string differentTimestampSignature = Client.GenerateSignature(baseBody, baseNonce, baseSecretKey, "9876543210", baseUri);
                Console.WriteLine($"   不同timestamp: {differentTimestampSignature}");
                
                // 测试不同uri
                string differentUriSignature = Client.GenerateSignature(baseBody, baseNonce, baseSecretKey, baseTimestamp, "/different/uri");
                Console.WriteLine($"   不同uri: {differentUriSignature}");
                
                // 验证不同参数应生成不同签名
                bool allDifferent = (baseSignature != differentBodySignature) &&
                                   (baseSignature != differentNonceSignature) &&
                                   (baseSignature != differentSecretKeySignature) &&
                                   (baseSignature != differentTimestampSignature) &&
                                   (baseSignature != differentUriSignature);
                
                if (allDifferent)
                {
                    Console.WriteLine("   ✅ 不同参数生成不同签名");
                    passedTests++;
                }
                else
                {
                    Console.WriteLine("   ❌ 不同参数生成了相同签名");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 测试失败: {ex.Message}");
            }
            
            // 输出测试结果汇总
            Console.WriteLine("\n==========================================");
            Console.WriteLine($"测试结果: {passedTests} / {totalTests} 测试通过");
            Console.WriteLine("==========================================");
            
            if (passedTests == totalTests)
            {
                Console.WriteLine("🎉 所有测试都通过了！");
            }
            else
            {
                Console.WriteLine("⚠️  有测试未通过，请检查代码。");
            }
        }
    }
}
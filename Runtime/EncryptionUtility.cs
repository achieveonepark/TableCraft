using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace com.achieve.scripting.table
{
    public class EncryptionUtility
    {
        private static string key = "kenhbjc5jregkfnb"; // 반드시 16, 24, 32바이트 길이여야 함 (AES는 키 크기가 중요)

        public static string Encrypt(string plainText)
        {
            byte[] compressedData = Compress(Encoding.UTF8.GetBytes(plainText));
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16]; // AES는 16바이트 IV(Initialization Vector)를 사용

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(compressedData, 0, compressedData.Length);
                        csEncrypt.FlushFinalBlock();
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16]; // 동일한 IV 사용
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var resultStream = new MemoryStream())
                        {
                            csDecrypt.CopyTo(resultStream);
                            // 압축 해제
                            byte[] decompressedData = Decompress(resultStream.ToArray());
                            return Encoding.UTF8.GetString(decompressedData);
                        }
                    }
                }
            }
        }

        // JSON 파일 암호화 저장
        public static void SaveEncryptedJson(string filePath, string jsonContent)
        {
            string encryptedContent = Encrypt(jsonContent);
            File.WriteAllText(filePath, encryptedContent);
            Debug.Log("Encrypted JSON saved at: " + filePath);
        }

        // JSON 파일 복호화 로드
        public static string LoadDecryptedJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptedContent = File.ReadAllText(filePath);
                return Decrypt(encryptedContent);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }
        }

        private static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        private static byte[] Decompress(byte[] compressedData)
        {
            using (var input = new MemoryStream(compressedData))
            {
                using (var output = new MemoryStream())
                {
                    using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                    {
                        gzip.CopyTo(output);
                    }
                    return output.ToArray();
                }
            }
        }
    }
}
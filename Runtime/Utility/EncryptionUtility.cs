#if ENABLE_ENCRYPT
using System;
using System.IO;
using UnityEngine;
using Achieve.DataProtector;

namespace Achieve.TableCraft
{
    public class EncryptionUtility
    {
        public static void SaveEncryptedJson(string filePath, string jsonContent, string key)
        {
            byte[] encryptedContent = Encryptor.Encrypt(jsonContent, key);
            File.WriteAllBytes(filePath, encryptedContent);
        }

        public static string LoadDecryptedJson(string filePath, string key)
        {
            if (File.Exists(filePath))
            {
                byte[] encryptedContent = File.ReadAllBytes(filePath);
                return Encryptor.DecryptToString(Convert.ToBase64String(encryptedContent), key);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }
        }
    }
}
#endif
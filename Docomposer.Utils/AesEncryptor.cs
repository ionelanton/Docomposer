using System;
using System.IO;
using System.Security.Cryptography;

namespace Docomposer.Utils
{
    public static class AesEncryptor
    {
        private static int KeySize = 256;
        
        public static string AesEncryptToBase64(string plainText, string key, string vector)
        {
            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {

                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = Convert.FromBase64String(vector);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string AesDecryptFromBase64(string cipherText, string key, string vector)
        {
            var plainText = "";

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = Convert.FromBase64String(vector);

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }

        public static (string Key, string IV) GenerateKeyAndIV()
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = KeySize;
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();
                return (Convert.ToBase64String(aesAlg.Key), Convert.ToBase64String(aesAlg.IV));
            }
        }
    }
}
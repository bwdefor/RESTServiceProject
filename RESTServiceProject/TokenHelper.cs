using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RESTServiceProject
{
    public class Token
    {
        public int UserId { get; set; }
        public DateTime Expires { get; set; }
    }

    public static class TokenHelper
    {
        public static string GetToken(string email, string password)
        {
            var token = new Token
            {
                UserId = 10,
                Expires = DateTime.UtcNow.AddMinutes(10),
            };
            var jsonString = JsonSerializer.Serialize(token);
            var encryptedJsonString = Crypto.EncryptStringAES(jsonString);
            return encryptedJsonString;
        }

        public static Token DecodeToken(string token)
        {
            var decryptedJsonString = Crypto.DecryptStringAES(token);
            var tokenObject = JsonSerializer.Deserialize<Token>(decryptedJsonString);
            if (tokenObject.Expires < DateTime.UtcNow)
            {
                return null;
            }
            return tokenObject;
        }
    }

    public class Crypto
    {
        private static readonly byte[] Salt =
            Encoding.ASCII.GetBytes("B78A07A7-14D8-4890-BC99-9145A14713C1");
        private const string Password = "sharedSecretPassword";

        public static string EncryptStringAES(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            string outStr;
            RijndaelManaged aesAlg = null;
            try
            {
                var key = new Rfc2898DeriveBytes(Password, Salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            return outStr;
        }


        public static string DecryptStringAES(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            RijndaelManaged aesAlg = null;
            string plaintext;
            try
            {
                var key = new Rfc2898DeriveBytes(Password, Salt);

                var bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                    aesAlg.IV = ReadByteArray(msDecrypt);

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }
            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }
            return buffer;
        }
    }
}
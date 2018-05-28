using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Yooocan.Logic.Utils.Shared
{
    /*
     * This class is shared, with the original at Yooocan.Logic.Shared.Utils.
     * It's used to enforce registration on yoocan for Alto referrals from yoocan (not security critical).
     * Please don't use it for something security critical, use ASP.NET Data Protection API instead.
     * Known weakness: the token is not signed!
     * */
    public class SharedUserTokenManager
    {
        private byte[] Key { get; }
        public SharedUserTokenManager(string base64Key)
        {
            Key = Convert.FromBase64String(base64Key);
        }

        public string CreateToken(string userName)
        {
            var token = new SharedUserToken
            {
                WasAuthorized = !string.IsNullOrEmpty(userName),
                UserName = userName,
                CreationDate = DateTimeOffset.UtcNow
            };

            var json = JsonConvert.SerializeObject(token);
            return EncryptString(json);
        }

        public SharedUserToken DecryptToken(string encrypted)
        {
            var json = DecryptString(encrypted);
            return JsonConvert.DeserializeObject<SharedUserToken>(json);
        }

        private string EncryptString(string text)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                using (var encryptor = aesAlg.CreateEncryptor())
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var encryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + encryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        private string DecryptString(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(Key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }

    public class SharedUserToken
    {
        public bool WasAuthorized { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}

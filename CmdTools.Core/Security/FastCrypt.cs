using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CmdTools.Core.Security
{
    public interface IFastCrypt
    {
        string EncryptString(string plainText);
        string DecryptString(string cipherText);
        SecureString DecryptStringAsSecureString(string cipherText);
    }

    public sealed class FastCrypt : IFastCrypt
    {
        private byte[] key;
        private byte[] iv;

        public FastCrypt()
        {
            int blockSize = 128;
            key = GetPasswordBytes("Slayer was an American thrash metal band from Huntington Park, California. The band was formed in 1981 by guitarists Kerry King and Jeff Hanneman, drummer Dave Lombardo, and bassist and vocalist Tom Araya. Slayer's fast and aggressive musical style made them one of the 'big four' bands of thrash metal, alongside Metallica, Megadeth, and Anthrax. Slayer's final lineup comprised King, Araya, drummer Paul Bostaph (who replaced Lombardo in 1992 and again in 2013) and guitarist Gary Holt (who replaced Hanneman in 2011). Drummer Jon Dette was also a member of the band.", blockSize);
            iv = GetPasswordBytes("Barbecue or barbeque (informally BBQ in the UK and US, barbie in Australia and braai in South Africa) is a term used with significant regional and national variations to describe various cooking methods which use live fire and smoke to cook the food.[1] The term is also generally applied to the devices associated with those methods, the broader cuisines that these methods produce, and the meals or gatherings at which this style of food is cooked and served. The cooking methods associated with barbecuing vary significantly but most involve outdoor cooking.", blockSize);
        }

        public string EncryptString(string plainText)
        {
            string base64CryptedStrign = String.Empty;

            using (ICryptoTransform encryptor = GetEnctriptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = memoryStream.ToArray();
                        base64CryptedStrign = Convert.ToBase64String(cipherTextBytes);
                    }
                }  
            }

            return base64CryptedStrign;
        }

        public string DecryptString(string cipherText)
        {
            string plainTextPassword = String.Empty;

            using (ICryptoTransform decryptor = GetDectriptor())
            {
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        plainTextPassword = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    }
                }
            }

            return plainTextPassword;
        }

        public SecureString DecryptStringAsSecureString(string cipherText)
        {
            string plainTextPassword = DecryptString(cipherText);

            SecureString securePassword = new SecureString();
            foreach (char c in plainTextPassword.ToArray())
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            return securePassword;
        }

        private byte[] GetPasswordBytes(string phrase, int numBits)
        {
            int numberOfBytes = numBits / 8;
            byte[] salt = new byte[8] { 102, 230, 69, 101, 12, 15, 146, 3 };
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(phrase, salt);
            byte[] keyBytes = password.GetBytes(numberOfBytes);
            return keyBytes;
        }

        private ICryptoTransform GetDectriptor()
        {
            AesCng symmetricKey = new AesCng();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(key, iv);
            return decryptor;
        }

        private ICryptoTransform GetEnctriptor()
        {
            AesCng symmetricKey = new AesCng();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(key, iv);
            return encryptor;
        }
    }
}

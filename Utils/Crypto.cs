using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace punk_tex_backend.Utils
{
    public static class Crypto
    {
        public const int SALT_BYTES = 128 / 8,
            PASSWORD_BYTES = 256 / 8;

        public static byte[] GenerateSalt() {
            var result = new byte[SALT_BYTES];
            using (var r = RandomNumberGenerator.Create()) {
                r.GetBytes(result);
            }
            return result;
        }

        public static string HashPassword(string password, byte[] salt = null) {
            if (salt == null) {
                salt = GenerateSalt();
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA1,
                10000,
                PASSWORD_BYTES
            )) + ";" + Convert.ToBase64String(salt);
        }
        
        public static bool CheckAgainstHash(string password, string hash) {
            var s = hash.Split(';', 2);
            var salt = Convert.FromBase64String(s[1]);
            return hash == HashPassword(password, salt);
        }
    }
}
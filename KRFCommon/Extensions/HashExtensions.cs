namespace KRFCommon.Extensions
{
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;

    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class HashExtensions
    {
        public static string GetSha1HashFromString(this string value)
        {
            return value.GetBytesFromString().GetSha1HashFromBytes();
        }

        public static string GetSha256HashFromString(this string value)
        {
            return value.GetBytesFromString().GetSha256HashFromBytes();
        }

        public static string GetSha512HashFromString(this string value)
        {
            return value.GetBytesFromString().GetSha512HashFromBytes();
        }

        public static string GetSaltedPassword(this string password, string username)
        {
            var salt = GenerateSalt();
            var saltedPw = password.GetHashFromPassword().SaltPassword(salt);
            return string.Concat(saltedPw.GetStringFromHashBytes(), ".", salt.XorPasswordSalt(username).GetStringFromHashBytes());
        }

        public static bool IsPasswordValid(this string password, string username, string saltedHashedPassword)
        {
            var hashAndSalt = saltedHashedPassword.Split('.');
            var cypherPw = Convert.FromBase64String(hashAndSalt[0]);
            var salt = Convert.FromBase64String(hashAndSalt[1]).XorPasswordSalt(username);
            var cypherPw2 = password.GetHashFromPassword().SaltPassword(salt);

            if (cypherPw == null || cypherPw2 == null || cypherPw.Length != cypherPw2.Length)
            {
                return false;
            }

            var areSame = true;

            for (var i = 0; i < cypherPw.Length; i++)
            {
                areSame &= (cypherPw[i] == cypherPw2[i]);
            }

            return areSame;
        }

        public static string GetSha1HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA1.Create())
            {
                return hash.ComputeHash(bytes).GetStringFromHashBytes();
            }
        }

        public static string GetSha256HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(bytes).GetStringFromHashBytes();
            }
        }

        public static string GetSha512HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA512.Create())
            {
                return hash.ComputeHash(bytes).GetStringFromHashBytes();
            }
        }

        private static byte[] GetBytesFromString(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private static string GetStringFromHashBytes(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private static string GetHashFromPassword(this string password)
        {
            using (var hash512 = SHA512.Create())
            {
                using (var hash256 = SHA256.Create())
                {
                    var part1 = hash256.ComputeHash(password.GetBytesFromString()).GetStringFromHashBytes();
                    var part2 = hash256.ComputeHash(password.ToUpperInvariant().GetBytesFromString()).GetStringFromHashBytes();

                    return hash512.ComputeHash(string.Concat(part1, part2).GetBytesFromString()).GetStringFromHashBytes();
                }
            }
        }

        private static byte[] XorPasswordSalt(this byte[] salt, string username)
        {
            var usernameHash = new byte[32];
            var xorSalt = new byte[32];

            using (var hash = SHA256.Create())
            {
                usernameHash = hash.ComputeHash(string.Concat(username).GetBytesFromString());
            }

            for (int i = 0; i < xorSalt.Length; i++)
            {
                xorSalt[i] = (byte)(salt[i] ^ usernameHash[i]);
            }

            return xorSalt;
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        private static byte[] SaltPassword(this string hashedPassword, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(hashedPassword, salt, KeyDerivationPrf.HMACSHA512, 30, 64);
        }
    }
}

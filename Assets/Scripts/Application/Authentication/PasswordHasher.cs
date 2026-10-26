
using System;
using System.Security.Cryptography;

namespace BlueCode3D.Application.Authentication
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 120000;

        public static void CreateHash(
            string password,
            out string passwordHash,
            out string passwordSalt)
        {
            byte[] salt = new byte[SaltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                passwordSalt = Convert.ToBase64String(salt);
                passwordHash = Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(
            string password,
            string storedHash,
            string storedSalt)
        {
            try
            {
                byte[] salt =
                    Convert.FromBase64String(storedSalt);

                byte[] expectedHash =
                    Convert.FromBase64String(storedHash);

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256))
                {
                    byte[] actualHash =
                        pbkdf2.GetBytes(expectedHash.Length);

                    return FixedTimeEquals(
                        actualHash,
                        expectedHash);
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static bool FixedTimeEquals(
            byte[] a,
            byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int difference = 0;

            for (int i = 0; i < a.Length; i++)
                difference |= a[i] ^ b[i];

            return difference == 0;
        }
    }
}

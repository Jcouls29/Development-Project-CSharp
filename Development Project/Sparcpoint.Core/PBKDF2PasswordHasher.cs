using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public class PBKDF2PasswordHasher : IPasswordHasher
    {
        public string AlgorithmName => "PBKDF2";

        public Task<(string, string)> CreateNewHashSet(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            return Task.FromResult((PerformHash(password, salt), Convert.ToBase64String(salt)));
        }

        public Task<string> HashPassword(string password, string salt)
        {
            return Task.FromResult(PerformHash(password, Convert.FromBase64String(salt)));
        }

        private string PerformHash(string password, byte[] salt) =>
            Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 11561,
                numBytesRequested: 256 / 8)
            );
    }
}

using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Tests.Core
{
    public class PBKDF2PasswordHasherTests
    {
        private readonly PBKDF2PasswordHasher _Hasher = new PBKDF2PasswordHasher();

        [Fact]
        public void AlgorithmName_IsPBKDF2()
        {
            Assert.Equal("PBKDF2", _Hasher.AlgorithmName);
        }

        [Fact]
        public async Task CreateNewHashSet_ReturnsNonEmptyHashAndSalt()
        {
            var (hash, salt) = await _Hasher.CreateNewHashSet("password123");

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.False(string.IsNullOrWhiteSpace(salt));
        }

        [Fact]
        public async Task CreateNewHashSet_SamePassword_ProducesDifferentSaltsEachTime()
        {
            var (_, salt1) = await _Hasher.CreateNewHashSet("same-password");
            var (_, salt2) = await _Hasher.CreateNewHashSet("same-password");

            // Random salt should differ between calls
            Assert.NotEqual(salt1, salt2);
        }

        [Fact]
        public async Task HashPassword_SamePasswordAndSalt_ProducesSameHash()
        {
            var (_, salt) = await _Hasher.CreateNewHashSet("myPassword");

            var hash1 = await _Hasher.HashPassword("myPassword", salt);
            var hash2 = await _Hasher.HashPassword("myPassword", salt);

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public async Task HashPassword_DifferentPasswords_ProduceDifferentHashes()
        {
            var (_, salt) = await _Hasher.CreateNewHashSet("any");

            var hash1 = await _Hasher.HashPassword("passwordA", salt);
            var hash2 = await _Hasher.HashPassword("passwordB", salt);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public async Task HashPassword_MatchesHashFromCreateNewHashSet()
        {
            const string password = "secure-password";
            var (originalHash, salt) = await _Hasher.CreateNewHashSet(password);

            var recomputed = await _Hasher.HashPassword(password, salt);

            Assert.Equal(originalHash, recomputed);
        }

        [Fact]
        public async Task Hash_IsBase64Encoded()
        {
            var (hash, _) = await _Hasher.CreateNewHashSet("test");

            // Should not throw — valid Base64 string
            var bytes = System.Convert.FromBase64String(hash);
            Assert.NotEmpty(bytes);
        }
    }
}

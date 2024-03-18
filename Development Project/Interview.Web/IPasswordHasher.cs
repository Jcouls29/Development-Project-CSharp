using System.Collections;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IPasswordHasher
    {
        string AlgorithmName { get; }
        Task<(string, string)> CreateNewHashSet(string password);
        Task<string> HashPassword(string password, string salt);
    }
}

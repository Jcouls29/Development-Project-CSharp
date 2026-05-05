using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IPasswordHasher
    {
        string AlgorithmName { get; }

        Task<(string, string)> CreateNewHashSet(string password);

        Task<string> HashPassword(string password, string salt);
    }
}
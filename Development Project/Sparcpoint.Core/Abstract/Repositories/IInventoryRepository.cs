using Sparcpoint.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Repositories
{
    public interface IInventoryRepository
    {
        Task UpdateInventoryAsync(List<UpdateInventoryRequestDto> request);

    }
}

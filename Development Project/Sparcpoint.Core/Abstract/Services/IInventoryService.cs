using Sparcpoint.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Services
{
    public interface IInventoryService
    {
        Task UpdateInventoryAsync(List<UpdateInventoryRequestDto> request);

    }
}

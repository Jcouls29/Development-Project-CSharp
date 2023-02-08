using AutoMapper;
using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    public class InventoryTransactionProfile : Profile
    {
        public InventoryTransactionProfile()
        {
            CreateMap<InventoryTransactionRequest, InventoryTransaction>();
        }
    }
}
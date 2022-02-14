using AutoMapper;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.Domain.Requestes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Profiles
{
    public class InventoryTransactionProfile : Profile
    {
        public InventoryTransactionProfile()
        {
            CreateMap<InventoryTransactionRequest, InventoryTransaction>();
        }
    }
}

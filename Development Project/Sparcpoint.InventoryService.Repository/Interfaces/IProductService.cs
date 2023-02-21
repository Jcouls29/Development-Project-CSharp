﻿using Sparcpoint.Inventory.Core.Requests;
using Sparcpoint.Inventory.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repository.Interfaces
{
    public interface IProductService
    {
        Task<int> AddProductAsync(AddProductRequest addProductRequest);
        Task<List<int>> AddProductAttributesAsync(List<AddProductAttributesRequest> addProductAttributesRequest);
    }
}

﻿using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Abstracts
{
    public interface IInventoryValidationService
    {
        ValidationResponse InventoryIsValid(InventoryTransactionRequest request);
    }
}
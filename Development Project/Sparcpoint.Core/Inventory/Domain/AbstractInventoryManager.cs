using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Product.Domain
{
    public abstract class AbstractInventoryManager
    {
        public abstract object[] GetInventoryItems();
    }
}

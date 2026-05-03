using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Requests
{
    //EVAL: Even though the add and remove requests have similar properties,
    //they are separate classes to maintain clear intent and separation of concerns.
    //This allows for potential future differences in validation or additional properties
    //specific to each action without affecting the other.
    public class AddInventoryRequest
    {
        public int InstanceId { get; set; }
        public int Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}

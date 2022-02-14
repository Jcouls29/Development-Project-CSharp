using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Responses
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string InvalidMessage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string InvalidMessage { get; set; }
    }
}

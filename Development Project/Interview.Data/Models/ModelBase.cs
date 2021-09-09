using System;

namespace Interview.Data.Models
{
    public class ModelBase
    {                
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}

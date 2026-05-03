using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Cateogries
    {
        public int Id { get; set; }
        public int CateogriesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; }

    }
}

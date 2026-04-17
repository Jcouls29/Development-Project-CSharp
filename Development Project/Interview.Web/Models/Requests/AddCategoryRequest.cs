using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    public class AddCategoryRequest
    {
        [Required, StringLength(64)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public IList<AttributePair> Attributes { get; set; } = new List<AttributePair>();

        public IList<int> ParentCategoryIds { get; set; } = new List<int>();
    }
}

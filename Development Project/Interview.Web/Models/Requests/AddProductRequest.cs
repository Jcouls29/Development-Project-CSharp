using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    /// <summary>
    /// EVAL: Input DTO — separates the API contract from the domain model
    /// to be able to evolve both independently (API versioning).
    /// </summary>
    public class AddProductRequest
    {
        [Required, StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public IList<string> ProductImageUris { get; set; } = new List<string>();

        public IList<string> ValidSkus { get; set; } = new List<string>();

        public IList<AttributePair> Attributes { get; set; } = new List<AttributePair>();

        public IList<int> CategoryIds { get; set; } = new List<int>();
    }

    public class AttributePair
    {
        [Required, StringLength(64)]
        public string Key { get; set; }

        [Required, StringLength(512)]
        public string Value { get; set; }
    }
}

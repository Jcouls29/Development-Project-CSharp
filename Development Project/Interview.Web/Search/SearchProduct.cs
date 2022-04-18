using System.Collections.Generic;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Search;

public class SearchProduct
{
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Metadata> SearchMetadatas { get; set; }
    public ICollection<Category> SearchCategories { get; set; }    
}
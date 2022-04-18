using System;

namespace Interview.Web.Domain.Entities;

public class CategoryHierarhy
{
    public Guid Id { get; set; }
    public Guid ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }
        
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}
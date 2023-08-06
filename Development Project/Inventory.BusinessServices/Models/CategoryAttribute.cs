namespace Inventory.BusinessServices
{
    public class CategoryAttribute
    {
        public int Id { get; set; }
        public Category? Category { get; set; }
        public int Key { get; set; }
        public string? Value { get; set; }
    }
}
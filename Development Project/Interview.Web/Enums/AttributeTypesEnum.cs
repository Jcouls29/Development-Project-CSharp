namespace Interview.Web.Enums
{
    // Has to be predefined and saved in database.
    // EF core migration would take care of making enums from database. 
    // Not in the scope of this assignment, but something to work on.
    public enum AttributeTypesEnum
    {
        Color,
        Size,
        Weight,
        Material,
        Brand,
        Model,
        Manufacturer,
        CountryOfOrigin,
        SKU, // Stock Keeping Unit
        UPC, // Universal Product Code
        EAN, // European Article Number
        MPN, // Manufacturer Part Number
        Condition,
        Warranty,
        Dimensions,
        Description,
        Features,
        Quantity,
        Price,

    }
}

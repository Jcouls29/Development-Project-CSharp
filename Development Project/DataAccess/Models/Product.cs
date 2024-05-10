using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class Product
{
    public int Id { get; init; }
    [MaxLength(256)]
    public string Name { get; init; } = string.Empty;
    [MaxLength(256)] public string Description { get; init; } = string.Empty;
    public DateTime CreatedTimestamp { get; init; }

    //EVAL: It's been a few months since I've had to do any heavy data schema setup in EF Core, so it's very possible
    // that my objects may not be fully connected in the most ideal way, or they may not accurately represent the existing table schema.
    // These aren't hard changes to make, including having explicit many-to-many tables to accurately represent the existing joins,
    // I just ran out of time to go through and be able to double-check all of these connections and/or make any corrections needed.
    public List<Category> Categories { get; init; } = new List<Category>();
    public List<Metadata> MetadataAttributes { get; init; } = new List<Metadata>();
}
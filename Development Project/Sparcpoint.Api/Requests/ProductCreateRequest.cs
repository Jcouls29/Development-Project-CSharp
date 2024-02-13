using Sparcpoint.Core.Entities;

namespace Sparcpoint.Api.Requests;

public record ProductCreateRequest
{
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string ImageUris { get; init; } = "";
    public string ValidSkus { get; init; } = "";

    public IEnumerable<CustomAttribute>? Attributes { get; init; }
    public IEnumerable<long>? Categories { get; init; }
}

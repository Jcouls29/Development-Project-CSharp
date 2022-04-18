using AutoMapper;
using Interview.Web.Domain.Dto;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Mappings;

public class Mapping : Profile
{
    public Mapping()
    {
        AllowNullCollections = true;
        MapEntitiesToDtos();
        MapDtosToEntities();
    }

    private void MapEntitiesToDtos()
    {
        CreateMap<Category, CategoryData>();
        CreateMap<MetadataType, MetadataTypeData>();
        CreateMap<Metadata, MetadataData>();
        CreateMap<Product, ProductData>();
        CreateMap<Inventory, InventoryData>();
        CreateMap<InventoryOperation, InventoryOperationData>();
    }
    
    private void MapDtosToEntities()
    {
        CreateMap<CategoryData, Category>();
        CreateMap<MetadataTypeData, MetadataType>();
        CreateMap<MetadataData, Metadata>();
        CreateMap<ProductData, Product>();
        CreateMap<InventoryData, Inventory>();
        CreateMap<InventoryOperationData, InventoryOperation>();
    }
}
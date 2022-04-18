using System;
using System.Collections.Generic;
using Interview.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Infrastructure;

public static class DataSeed
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        var DairyCategory = new Category
        {
            Id = new Guid("8e014c1b-d4cc-4e05-b346-ede89f513a44"),
            Name = "Dairy",
            Description = "Category with dairy products"
        };
        var BeveragesCategory = new Category
        {
            Id = new Guid("d4bc694b-b9f3-469c-ae4d-75b41905c4a8"),
            Name = "Beverages",
            Description = "Beverage products"
        };
        var SodaCategory = new Category
        {
            Id = new Guid("ecc25b3b-f90d-4d6d-a950-44e8c53f8929"),
            Name = "Soda",
            Description = "Soda drinks"
        };

        modelBuilder.Entity<Category>().HasData(
            DairyCategory, BeveragesCategory, SodaCategory);


        var SkuMetadataType = new MetadataType
        {
            Id = new Guid("479b595b-f4a3-4162-ac57-89897f3a5b95"),
            Name = "SKU"
        };
        var ColorMetadataType = new MetadataType
        {
            Id = new Guid("3ab03e92-4ebe-4c5e-887d-d7a96e3ffa0b"),
            Name = "Color"
        };
        var SizeMetadataType = new MetadataType
        {
            Id = new Guid("86dd6aef-b4d5-4b2e-bb5e-221aa12dc8e3"),
            Name = "Size"
        };
        var ManufacturerMetadataType = new MetadataType
        {
            Id = new Guid("1db713ec-9c06-472d-bfa9-b08ae0bcb0c7"),
            Name = "Manufacturer"
        };

        modelBuilder.Entity<MetadataType>().HasData(
            SkuMetadataType, ColorMetadataType, SizeMetadataType, ManufacturerMetadataType
        );

        var BlueColorMetadata = new Metadata
        {
            Id = new Guid("21a1c630-fa76-4d90-85fe-a94590975a74"),
            MetadataTypeId = ColorMetadataType.Id,
            Value = "Blue",
            Name = "Blue Color metadata",
            Description = "Color metadata with blue value"
        };

        var LargeSizeMetadata = new Metadata
        {
            Id = new Guid("101944a3-a57a-4336-bb49-0fc130cb9777"),
            MetadataTypeId = SizeMetadataType.Id,
            Value = "Large"
        };

        var SampleSkuMetadata = new Metadata
        {
            Id = new Guid("2b2e4769-04aa-40d6-bb49-8377350d7b54"),
            MetadataTypeId = SkuMetadataType.Id,
            Value = "135-009-12123",
            Name = "Soda Tonic SKU"
        };
        
        var SampleManufacturerMetadata = new Metadata
        {
            Id = new Guid("e291a1b4-ef30-4d73-8d4b-5464b52645e3"),
            MetadataTypeId = ManufacturerMetadataType.Id,
            Value = "Tonic Guys Inc.",
        };
        
        var AnotherManufacturerMetadata = new Metadata
        {
            Id = new Guid("138cc3b1-7e38-417b-88ea-f93e7821f22d"),
            MetadataTypeId = ManufacturerMetadataType.Id,
            Value = "Coca-Cola beverages Inc.",
        };

        modelBuilder.Entity<Metadata>().HasData(
            BlueColorMetadata, LargeSizeMetadata, SampleSkuMetadata, SampleManufacturerMetadata, AnotherManufacturerMetadata);

        var MediumBottleUnit = new Unit
        {
            Id = new Guid("07eba7d1-32df-4ce8-8174-a799894bb7d3"),
            Description = "20 Fl Oz bottle",
            DisplayName = "20 Fl Oz",
            Name = "20 Fl Oz bottle"
        };

        var LargeBottleUnit = new Unit
        {
            Id = new Guid("87dc6e3c-705c-4cb5-8383-1c0585153aa5"),
            Description = "50 Fl Oz bottle",
            DisplayName = "50 Fl Oz",
            Name = "50 Fl Oz bottle"
        };

        modelBuilder.Entity<Unit>().HasData(MediumBottleUnit, LargeBottleUnit);

        var tonicProductId = new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a");
        var cokeProductId = new Guid("9192ddf4-5d38-4873-9412-008c43f6c057");
        
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = tonicProductId,
                Name = "Tonic Soda",
                Description = "Medium bottle of tonic Soda from Tonic Guys. High level of carbonation",
                UnitId = MediumBottleUnit.Id,
            },
            new Product
            {
                Id = cokeProductId,
                Name = "Cherry Coke",
                Description = "Medium bottle of cherry coke from Coca-Cola.",
                UnitId = MediumBottleUnit.Id,
            });

        modelBuilder
            .Entity<Product>()
            .HasMany(p => p.Categories)
            .WithMany(p => p.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductsCategories",
                r => r.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                l => l.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                je =>
                {
                    je.HasKey("ProductId", "CategoryId");
                    je.HasData(
                        new { ProductId = tonicProductId, CategoryId = BeveragesCategory.Id },
                        new { ProductId = tonicProductId, CategoryId = SodaCategory.Id },
                        new { ProductId = cokeProductId, CategoryId = BeveragesCategory.Id }
                    );
                }
            );

        modelBuilder
            .Entity<Product>()
            .HasMany(p => p.Metadatas)
            .WithMany(p => p.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductsMetadatas",
                r => r.HasOne<Metadata>().WithMany().HasForeignKey("MetadataId"),
                l => l.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                je =>
                {
                    je.HasKey("ProductId", "MetadataId");
                    je.HasData(
                        new { ProductId = tonicProductId, MetadataId = SampleSkuMetadata.Id },
                        new { ProductId = tonicProductId, MetadataId = SampleManufacturerMetadata.Id},
                        new { ProductId = cokeProductId, MetadataId = AnotherManufacturerMetadata.Id}
                        );
                }
            );
    }
}
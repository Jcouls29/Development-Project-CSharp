using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetadataTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryHierarhy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryHierarhy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryHierarhy_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryHierarhy_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetadataTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metadatas_MetadataTypes_MetadataTypeId",
                        column: x => x.MetadataTypeId,
                        principalTable: "MetadataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductInventoryLocation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ProductInventoryDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsCategories",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsCategories", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductsCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsMetadatas",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetadataId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsMetadatas", x => new { x.ProductId, x.MetadataId });
                    table.ForeignKey(
                        name: "FK_ProductsMetadatas_Metadatas_MetadataId",
                        column: x => x.MetadataId,
                        principalTable: "Metadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsMetadatas_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Started = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Completed = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryOperations_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("8e014c1b-d4cc-4e05-b346-ede89f513a44"), "Category with dairy products", "Dairy" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("d4bc694b-b9f3-469c-ae4d-75b41905c4a8"), "Beverage products", "Beverages" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("ecc25b3b-f90d-4d6d-a950-44e8c53f8929"), "Soda drinks", "Soda" });

            migrationBuilder.InsertData(
                table: "MetadataTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("1db713ec-9c06-472d-bfa9-b08ae0bcb0c7"), "Manufacturer" });

            migrationBuilder.InsertData(
                table: "MetadataTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("3ab03e92-4ebe-4c5e-887d-d7a96e3ffa0b"), "Color" });

            migrationBuilder.InsertData(
                table: "MetadataTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("479b595b-f4a3-4162-ac57-89897f3a5b95"), "SKU" });

            migrationBuilder.InsertData(
                table: "MetadataTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("86dd6aef-b4d5-4b2e-bb5e-221aa12dc8e3"), "Size" });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Description", "DisplayName", "Name" },
                values: new object[] { new Guid("07eba7d1-32df-4ce8-8174-a799894bb7d3"), "20 Fl Oz bottle", "20 Fl Oz", "20 Fl Oz bottle" });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Description", "DisplayName", "Name" },
                values: new object[] { new Guid("87dc6e3c-705c-4cb5-8383-1c0585153aa5"), "50 Fl Oz bottle", "50 Fl Oz", "50 Fl Oz bottle" });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "Id", "Description", "MetadataTypeId", "Name", "Value" },
                values: new object[] { new Guid("101944a3-a57a-4336-bb49-0fc130cb9777"), null, new Guid("86dd6aef-b4d5-4b2e-bb5e-221aa12dc8e3"), null, "Large" });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "Id", "Description", "MetadataTypeId", "Name", "Value" },
                values: new object[] { new Guid("138cc3b1-7e38-417b-88ea-f93e7821f22d"), null, new Guid("1db713ec-9c06-472d-bfa9-b08ae0bcb0c7"), null, "Coca-Cola beverages Inc." });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "Id", "Description", "MetadataTypeId", "Name", "Value" },
                values: new object[] { new Guid("21a1c630-fa76-4d90-85fe-a94590975a74"), "Color metadata with blue value", new Guid("3ab03e92-4ebe-4c5e-887d-d7a96e3ffa0b"), "Blue Color metadata", "Blue" });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "Id", "Description", "MetadataTypeId", "Name", "Value" },
                values: new object[] { new Guid("2b2e4769-04aa-40d6-bb49-8377350d7b54"), null, new Guid("479b595b-f4a3-4162-ac57-89897f3a5b95"), "Soda Tonic SKU", "135-009-12123" });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "Id", "Description", "MetadataTypeId", "Name", "Value" },
                values: new object[] { new Guid("e291a1b4-ef30-4d73-8d4b-5464b52645e3"), null, new Guid("1db713ec-9c06-472d-bfa9-b08ae0bcb0c7"), null, "Tonic Guys Inc." });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "UnitId" },
                values: new object[] { new Guid("9192ddf4-5d38-4873-9412-008c43f6c057"), "Medium bottle of cherry coke from Coca-Cola.", "Cherry Coke", new Guid("07eba7d1-32df-4ce8-8174-a799894bb7d3") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "UnitId" },
                values: new object[] { new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a"), "Medium bottle of tonic Soda from Tonic Guys. High level of carbonation", "Tonic Soda", new Guid("07eba7d1-32df-4ce8-8174-a799894bb7d3") });

            migrationBuilder.InsertData(
                table: "ProductsCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[] { new Guid("d4bc694b-b9f3-469c-ae4d-75b41905c4a8"), new Guid("9192ddf4-5d38-4873-9412-008c43f6c057") });

            migrationBuilder.InsertData(
                table: "ProductsCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[] { new Guid("d4bc694b-b9f3-469c-ae4d-75b41905c4a8"), new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a") });

            migrationBuilder.InsertData(
                table: "ProductsCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[] { new Guid("ecc25b3b-f90d-4d6d-a950-44e8c53f8929"), new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a") });

            migrationBuilder.InsertData(
                table: "ProductsMetadatas",
                columns: new[] { "MetadataId", "ProductId" },
                values: new object[] { new Guid("138cc3b1-7e38-417b-88ea-f93e7821f22d"), new Guid("9192ddf4-5d38-4873-9412-008c43f6c057") });

            migrationBuilder.InsertData(
                table: "ProductsMetadatas",
                columns: new[] { "MetadataId", "ProductId" },
                values: new object[] { new Guid("2b2e4769-04aa-40d6-bb49-8377350d7b54"), new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a") });

            migrationBuilder.InsertData(
                table: "ProductsMetadatas",
                columns: new[] { "MetadataId", "ProductId" },
                values: new object[] { new Guid("e291a1b4-ef30-4d73-8d4b-5464b52645e3"), new Guid("fbd829e7-6251-4724-97bd-76b3c2427e7a") });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryHierarhy_CategoryId",
                table: "CategoryHierarhy",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryHierarhy_ParentCategoryId",
                table: "CategoryHierarhy",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId",
                table: "Inventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperations_InventoryId",
                table: "InventoryOperations",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Metadatas_MetadataTypeId",
                table: "Metadatas",
                column: "MetadataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitId",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsCategories_CategoryId",
                table: "ProductsCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsMetadatas_MetadataId",
                table: "ProductsMetadatas",
                column: "MetadataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryHierarhy");

            migrationBuilder.DropTable(
                name: "InventoryOperations");

            migrationBuilder.DropTable(
                name: "ProductsCategories");

            migrationBuilder.DropTable(
                name: "ProductsMetadatas");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Metadatas");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "MetadataTypes");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}

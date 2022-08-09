using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sparcpoint.DataLayer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.InstanceId);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductImageUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidSkus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.InstanceId);
                });

            migrationBuilder.CreateTable(
                name: "CategoryOfCategory",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryOfCategory", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_CategoryOfCategory_Categories_CategoryInstanceId",
                        column: x => x.CategoryInstanceId,
                        principalTable: "Categories",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatergoryAttributes",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatergoryAttributes", x => new { x.InstanceId, x.Key });
                    table.ForeignKey(
                        name: "FK_CatergoryAttributes_Categories_CategoryInstanceId",
                        column: x => x.CategoryInstanceId,
                        principalTable: "Categories",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductsInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => new { x.InstanceId, x.Key });
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Products_ProductsInstanceId",
                        column: x => x.ProductsInstanceId,
                        principalTable: "Products",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.InstanceId, x.CategoryInstanceId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoryInstanceId",
                        column: x => x.CategoryInstanceId,
                        principalTable: "Categories",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductsInstanceId",
                        column: x => x.ProductsInstanceId,
                        principalTable: "Products",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryOfCategory_CategoryInstanceId",
                table: "CategoryOfCategory",
                column: "CategoryInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CatergoryAttributes_CategoryInstanceId",
                table: "CatergoryAttributes",
                column: "CategoryInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductsInstanceId",
                table: "ProductAttribute",
                column: "ProductsInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryInstanceId",
                table: "ProductCategories",
                column: "CategoryInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductsInstanceId",
                table: "ProductCategories",
                column: "ProductsInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryOfCategory");

            migrationBuilder.DropTable(
                name: "CatergoryAttributes");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}

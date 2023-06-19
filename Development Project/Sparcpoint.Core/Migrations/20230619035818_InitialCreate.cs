using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Sparcpoint.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Instances");

            migrationBuilder.EnsureSchema(
                name: "Transactions");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.InstanceId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    ProductImageUris = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    ValidSkus = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.InstanceId);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAttributes",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributes", x => new { x.InstanceId, x.Key });
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_Categories",
                        column: x => x.InstanceId,
                        principalSchema: "Instances",
                        principalTable: "Categories",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryCategories",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    CategoryInstanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCategories", x => new { x.InstanceId, x.CategoryInstanceId });
                    table.ForeignKey(
                        name: "FK_CategoryCategories_Categories",
                        column: x => x.InstanceId,
                        principalSchema: "Instances",
                        principalTable: "Categories",
                        principalColumn: "InstanceId");
                    table.ForeignKey(
                        name: "FK_CategoryCategories_Categories_Categories",
                        column: x => x.CategoryInstanceId,
                        principalSchema: "Instances",
                        principalTable: "Categories",
                        principalColumn: "InstanceId");
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                schema: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductInstanceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    StartedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CompletedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeCategory = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Products",
                        column: x => x.ProductInstanceId,
                        principalSchema: "Instances",
                        principalTable: "Products",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => new { x.InstanceId, x.Key });
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products",
                        column: x => x.InstanceId,
                        principalSchema: "Instances",
                        principalTable: "Products",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "Instances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    CategoryInstanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.InstanceId, x.CategoryInstanceId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories",
                        column: x => x.CategoryInstanceId,
                        principalSchema: "Instances",
                        principalTable: "Categories",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products",
                        column: x => x.InstanceId,
                        principalSchema: "Instances",
                        principalTable: "Products",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategories_CategoryInstanceId",
                schema: "Instances",
                table: "CategoryCategories",
                column: "CategoryInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_CompletedTimestamp",
                schema: "Transactions",
                table: "InventoryTransactions",
                column: "CompletedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductInstanceId",
                schema: "Transactions",
                table: "InventoryTransactions",
                column: "ProductInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductInstanceId_Quantity",
                schema: "Transactions",
                table: "InventoryTransactions",
                columns: new[] { "ProductInstanceId", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_Key_Value",
                schema: "Instances",
                table: "ProductAttributes",
                columns: new[] { "Key", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryInstanceId",
                schema: "Instances",
                table: "ProductCategories",
                column: "CategoryInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAttributes",
                schema: "Instances");

            migrationBuilder.DropTable(
                name: "CategoryCategories",
                schema: "Instances");

            migrationBuilder.DropTable(
                name: "InventoryTransactions",
                schema: "Transactions");

            migrationBuilder.DropTable(
                name: "ProductAttributes",
                schema: "Instances");

            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "Instances");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Instances");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Instances");
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartERP.Inventory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAndPurchasingOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationalRecords",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    DestinationWarehouseId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    MinimumStockLevel = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DocumentDateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpectedDateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpirationDateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationalRecords_CompanyId_ExpirationDateUtc",
                schema: "inventory",
                table: "OperationalRecords",
                columns: new[] { "CompanyId", "ExpirationDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationalRecords_CompanyId_ProductId_WarehouseId",
                schema: "inventory",
                table: "OperationalRecords",
                columns: new[] { "CompanyId", "ProductId", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationalRecords_CompanyId_Type_Code",
                schema: "inventory",
                table: "OperationalRecords",
                columns: new[] { "CompanyId", "Type", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationalRecords_CompanyId_Type_Status",
                schema: "inventory",
                table: "OperationalRecords",
                columns: new[] { "CompanyId", "Type", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationalRecords",
                schema: "inventory");
        }
    }
}

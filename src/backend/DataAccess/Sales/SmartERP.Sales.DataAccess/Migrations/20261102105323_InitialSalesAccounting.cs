using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartERP.Sales.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialSalesAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.CreateTable(
                name: "SalesFinancialRecords",
                schema: "sales",
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
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    AccountId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DocumentDateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DueDateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PeriodStartUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PeriodEndUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesFinancialRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesFinancialRecords_CompanyId_AccountId",
                schema: "sales",
                table: "SalesFinancialRecords",
                columns: new[] { "CompanyId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesFinancialRecords_CompanyId_CustomerId",
                schema: "sales",
                table: "SalesFinancialRecords",
                columns: new[] { "CompanyId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesFinancialRecords_CompanyId_DueDateUtc",
                schema: "sales",
                table: "SalesFinancialRecords",
                columns: new[] { "CompanyId", "DueDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesFinancialRecords_CompanyId_Type_Code",
                schema: "sales",
                table: "SalesFinancialRecords",
                columns: new[] { "CompanyId", "Type", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesFinancialRecords_CompanyId_Type_Status",
                schema: "sales",
                table: "SalesFinancialRecords",
                columns: new[] { "CompanyId", "Type", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesFinancialRecords",
                schema: "sales");
        }
    }
}

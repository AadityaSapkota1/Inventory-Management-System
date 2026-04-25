using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VehicleManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToNewSchemaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendors_Vendor_Id",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_Vendor_Id",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "Contact_Person",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Vendor_Id",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Vendors",
                newName: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Status",
                table: "PurchaseInvoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "VendorItems",
                columns: table => new
                {
                    VendorItem_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Part_Name = table.Column<string>(type: "text", nullable: false),
                    Part_Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    Vendor_Id = table.Column<int>(type: "integer", nullable: false),
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorItems", x => x.VendorItem_Id);
                    table.ForeignKey(
                        name: "FK_VendorItems_Parts_Part_Id",
                        column: x => x.Part_Id,
                        principalTable: "Parts",
                        principalColumn: "Part_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorItems_Vendors_Vendor_Id",
                        column: x => x.Vendor_Id,
                        principalTable: "Vendors",
                        principalColumn: "Vendor_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorItems_Part_Id",
                table: "VendorItems",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_VendorItems_Vendor_Id",
                table: "VendorItems",
                column: "Vendor_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorItems");

            migrationBuilder.DropColumn(
                name: "Payment_Status",
                table: "PurchaseInvoices");

            migrationBuilder.RenameColumn(
                name: "Contact",
                table: "Vendors",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Contact_Person",
                table: "Vendors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Vendors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Vendor_Id",
                table: "Parts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Vendor_Id",
                table: "Parts",
                column: "Vendor_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Vendors_Vendor_Id",
                table: "Parts",
                column: "Vendor_Id",
                principalTable: "Vendors",
                principalColumn: "Vendor_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

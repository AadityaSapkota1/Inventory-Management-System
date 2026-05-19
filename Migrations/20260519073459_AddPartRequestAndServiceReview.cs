using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VehicleManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPartRequestAndServiceReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartRequests_Parts_Part_Id",
                table: "PartRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Appointments_Appointment_Id",
                table: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_SalesInvoices_Appointment_Id",
                table: "SalesInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PartRequests_Part_Id",
                table: "PartRequests");

            migrationBuilder.DropColumn(
                name: "Appointment_Id",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "Part_Id",
                table: "PartRequests");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PartRequests",
                newName: "Part_Name");

            migrationBuilder.RenameColumn(
                name: "Part_Request_Id",
                table: "PartRequests",
                newName: "Request_Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sales_Date",
                table: "SalesInvoices",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Purchase_Date",
                table: "PurchaseInvoices",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Request_Date",
                table: "PartRequests",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PartRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Notification_Time",
                table: "Notifications",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Service_Date",
                table: "Appointments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "ServiceReviews",
                columns: table => new
                {
                    Review_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Review_Text = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Review_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReviews", x => x.Review_Id);
                    table.ForeignKey(
                        name: "FK_ServiceReviews_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReviews_User_Id",
                table: "ServiceReviews",
                column: "User_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PartRequests");

            migrationBuilder.RenameColumn(
                name: "Part_Name",
                table: "PartRequests",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Request_Id",
                table: "PartRequests",
                newName: "Part_Request_Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sales_Date",
                table: "SalesInvoices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "Appointment_Id",
                table: "SalesInvoices",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Purchase_Date",
                table: "PurchaseInvoices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Request_Date",
                table: "PartRequests",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "Part_Id",
                table: "PartRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Notification_Time",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Service_Date",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Quotation_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Appointment_Id = table.Column<int>(type: "integer", nullable: false),
                    Labor_Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quotation_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Quotation_Id);
                    table.ForeignKey(
                        name: "FK_Quotations_Appointments_Appointment_Id",
                        column: x => x.Appointment_Id,
                        principalTable: "Appointments",
                        principalColumn: "Appointment_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    QuotationItem_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Part_Id = table.Column<int>(type: "integer", nullable: false),
                    Quotation_Id = table.Column<int>(type: "integer", nullable: false),
                    Is_Selected = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItems", x => x.QuotationItem_Id);
                    table.ForeignKey(
                        name: "FK_QuotationItems_Parts_Part_Id",
                        column: x => x.Part_Id,
                        principalTable: "Parts",
                        principalColumn: "Part_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationItems_Quotations_Quotation_Id",
                        column: x => x.Quotation_Id,
                        principalTable: "Quotations",
                        principalColumn: "Quotation_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_Appointment_Id",
                table: "SalesInvoices",
                column: "Appointment_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartRequests_Part_Id",
                table: "PartRequests",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_Part_Id",
                table: "QuotationItems",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_Quotation_Id",
                table: "QuotationItems",
                column: "Quotation_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_Appointment_Id",
                table: "Quotations",
                column: "Appointment_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartRequests_Parts_Part_Id",
                table: "PartRequests",
                column: "Part_Id",
                principalTable: "Parts",
                principalColumn: "Part_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Appointments_Appointment_Id",
                table: "SalesInvoices",
                column: "Appointment_Id",
                principalTable: "Appointments",
                principalColumn: "Appointment_Id");
        }
    }
}

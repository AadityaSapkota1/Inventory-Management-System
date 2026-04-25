using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VehicleManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    User_Role = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Contact = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Vendor_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vendor_Name = table.Column<string>(type: "text", nullable: false),
                    Contact_Person = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Vendor_Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notification_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Notification_Message = table.Column<string>(type: "text", nullable: false),
                    Notification_Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Notification_Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Vehicle_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vehicle_Number = table.Column<string>(type: "text", nullable: false),
                    Vehicle_Type = table.Column<string>(type: "text", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Vehicle_Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Part_Name = table.Column<string>(type: "text", nullable: false),
                    Part_Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    Vendor_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Part_Id);
                    table.ForeignKey(
                        name: "FK_Parts_Vendors_Vendor_Id",
                        column: x => x.Vendor_Id,
                        principalTable: "Vendors",
                        principalColumn: "Vendor_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    Purchase_Invoice_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Purchase_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total_Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Vendor_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.Purchase_Invoice_ID);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Vendors_Vendor_Id",
                        column: x => x.Vendor_Id,
                        principalTable: "Vendors",
                        principalColumn: "Vendor_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Appointment_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Service_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Service_Time = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false),
                    Vehicle_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Appointment_Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Vehicles_Vehicle_Id",
                        column: x => x.Vehicle_Id,
                        principalTable: "Vehicles",
                        principalColumn: "Vehicle_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartRequests",
                columns: table => new
                {
                    Part_Request_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Request_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false),
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartRequests", x => x.Part_Request_Id);
                    table.ForeignKey(
                        name: "FK_PartRequests_Parts_Part_Id",
                        column: x => x.Part_Id,
                        principalTable: "Parts",
                        principalColumn: "Part_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartRequests_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseItems",
                columns: table => new
                {
                    PurchaseItem_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Purchase_Invoice_ID = table.Column<int>(type: "integer", nullable: false),
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseItems", x => x.PurchaseItem_Id);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Parts_Part_Id",
                        column: x => x.Part_Id,
                        principalTable: "Parts",
                        principalColumn: "Part_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_PurchaseInvoices_Purchase_Invoice_ID",
                        column: x => x.Purchase_Invoice_ID,
                        principalTable: "PurchaseInvoices",
                        principalColumn: "Purchase_Invoice_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Quotation_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quotation_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Labor_Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Appointment_Id = table.Column<int>(type: "integer", nullable: false)
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
                name: "SalesInvoices",
                columns: table => new
                {
                    Sales_Invoice_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sales_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total_Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Payment_Status = table.Column<string>(type: "text", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: false),
                    Appointment_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoices", x => x.Sales_Invoice_ID);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Appointments_Appointment_Id",
                        column: x => x.Appointment_Id,
                        principalTable: "Appointments",
                        principalColumn: "Appointment_Id");
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    QuotationItem_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Is_Selected = table.Column<bool>(type: "boolean", nullable: false),
                    Quotation_Id = table.Column<int>(type: "integer", nullable: false),
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SalesItems",
                columns: table => new
                {
                    SalesItem_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Sales_Invoice_ID = table.Column<int>(type: "integer", nullable: false),
                    Part_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesItems", x => x.SalesItem_Id);
                    table.ForeignKey(
                        name: "FK_SalesItems_Parts_Part_Id",
                        column: x => x.Part_Id,
                        principalTable: "Parts",
                        principalColumn: "Part_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesItems_SalesInvoices_Sales_Invoice_ID",
                        column: x => x.Sales_Invoice_ID,
                        principalTable: "SalesInvoices",
                        principalColumn: "Sales_Invoice_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_User_Id",
                table: "Appointments",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Vehicle_Id",
                table: "Appointments",
                column: "Vehicle_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_User_Id",
                table: "Notifications",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartRequests_Part_Id",
                table: "PartRequests",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartRequests_User_Id",
                table: "PartRequests",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Vendor_Id",
                table: "Parts",
                column: "Vendor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_Vendor_Id",
                table: "PurchaseInvoices",
                column: "Vendor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_Part_Id",
                table: "PurchaseItems",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_Purchase_Invoice_ID",
                table: "PurchaseItems",
                column: "Purchase_Invoice_ID");

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

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_Appointment_Id",
                table: "SalesInvoices",
                column: "Appointment_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_User_Id",
                table: "SalesInvoices",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_Part_Id",
                table: "SalesItems",
                column: "Part_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_Sales_Invoice_ID",
                table: "SalesItems",
                column: "Sales_Invoice_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_User_Id",
                table: "Vehicles",
                column: "User_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PartRequests");

            migrationBuilder.DropTable(
                name: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "SalesItems");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

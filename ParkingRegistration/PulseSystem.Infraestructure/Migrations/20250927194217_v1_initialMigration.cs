using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseSystem.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class v1_initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PARKINGS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Complement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvailableArea = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Capacity = table.Column<int>(type: "INT", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PARKINGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GATEWAYS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    MacAddress = table.Column<string>(type: "VARCHAR(17)", maxLength: 17, nullable: false),
                    LastIP = table.Column<string>(type: "VARCHAR(15)", maxLength: 15, nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ParkingId = table.Column<long>(type: "bigint", nullable: false),
                    MaxCoverageArea = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    MaxCapacity = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GATEWAYS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GATEWAYS_PARKINGS_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "PARKINGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZONES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false),
                    Width = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    ParkingId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZONES_PARKINGS_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "PARKINGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GATEWAYS_ParkingId",
                table: "GATEWAYS",
                column: "ParkingId");

            migrationBuilder.CreateIndex(
                name: "IX_ZONES_ParkingId",
                table: "ZONES",
                column: "ParkingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GATEWAYS");

            migrationBuilder.DropTable(
                name: "ZONES");

            migrationBuilder.DropTable(
                name: "PARKINGS");
        }
    }
}

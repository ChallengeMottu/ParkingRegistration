using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseSystem.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class v1_initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "PARKINGS",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "VARCHAR2(150)", maxLength: 150, nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    COMPLEMENT = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NEIGHBORHOOD = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CEP = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    CITY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    AVAILABLE_AREA = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CAPACITY = table.Column<decimal>(type: "NUMBER", nullable: false),
                    REGISTER_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE"),
                    STRUCTURE_PLAN = table.Column<string>(type: "VARCHAR2(4000)", nullable: false),
                    FLOOR_PLAN = table.Column<string>(type: "VARCHAR2(4000)", nullable: false),
                    MAP_PLAN = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PARKINGS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GATEWAYS",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MODEL = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<decimal>(type: "NUMBER", nullable: false),
                    MAC_ADDRESS = table.Column<string>(type: "VARCHAR2(17)", maxLength: 17, nullable: false),
                    LAST_IP = table.Column<string>(type: "VARCHAR2(15)", maxLength: 15, nullable: false),
                    REGISTER_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE"),
                    ParkingId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    MAX_COVERAGE_AREA = table.Column<decimal>(type: "NUMBER", nullable: false),
                    MAX_CAPACITY = table.Column<decimal>(type: "NUMBER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GATEWAYS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GATEWAYS_PARKINGS_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "PARKINGS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZONES",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "VARCHAR2(500)", maxLength: 500, nullable: false),
                    WIDTH = table.Column<decimal>(type: "NUMBER", nullable: false),
                    LENGTH = table.Column<decimal>(type: "NUMBER", nullable: false),
                    ParkingId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZONES_PARKINGS_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "PARKINGS",
                        principalColumn: "ID",
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

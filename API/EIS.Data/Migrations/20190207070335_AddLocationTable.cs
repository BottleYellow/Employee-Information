using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EIS.Data.Migrations
{
    public partial class AddLocationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                schema: "LMS",
                table: "tblPerson",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tblLocations",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLocations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_LocationId",
                schema: "LMS",
                table: "tblPerson",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblPerson_tblLocations_LocationId",
                schema: "LMS",
                table: "tblPerson",
                column: "LocationId",
                principalSchema: "LMS",
                principalTable: "tblLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblPerson_tblLocations_LocationId",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropTable(
                name: "tblLocations",
                schema: "LMS");

            migrationBuilder.DropIndex(
                name: "IX_tblPerson_LocationId",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "LMS",
                table: "tblPerson");
        }
    }
}

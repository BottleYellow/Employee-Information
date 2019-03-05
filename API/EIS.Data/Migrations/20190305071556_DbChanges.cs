using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EIS.Data.Migrations
{
    public partial class DbChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnProbation",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropbationPeriodInMonth",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeeklyOffId",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingHours",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblWeeklyOffs",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWeeklyOffs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_WeeklyOffId",
                schema: "LMS",
                table: "tblPerson",
                column: "WeeklyOffId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblPerson_tblWeeklyOffs_WeeklyOffId",
                schema: "LMS",
                table: "tblPerson",
                column: "WeeklyOffId",
                principalSchema: "LMS",
                principalTable: "tblWeeklyOffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblPerson_tblWeeklyOffs_WeeklyOffId",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropTable(
                name: "tblWeeklyOffs",
                schema: "LMS");

            migrationBuilder.DropIndex(
                name: "IX_tblPerson_WeeklyOffId",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "IsOnProbation",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "PropbationPeriodInMonth",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "WeeklyOffId",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                schema: "LMS",
                table: "tblPerson");
        }
    }
}

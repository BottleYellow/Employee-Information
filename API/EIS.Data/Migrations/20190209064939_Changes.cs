using Microsoft.EntityFrameworkCore.Migrations;

namespace EIS.Data.Migrations
{
    public partial class Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblRoles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblRoles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPerson",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPermanentAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPermanentAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPastLeaves",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPastLeaves",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblOtherAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblOtherAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLocations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLocations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveRules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveRules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                schema: "LMS",
                table: "tblLeaveRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveCredit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveCredit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblHolidays",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblHolidays",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblEmployeeLeaves",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblEmployeeLeaves",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblEmergencyAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblEmergencyAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblCurrentAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblCurrentAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblAttendance",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblAttendance",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPerson");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPermanentAddress");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPermanentAddress");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblPastLeaves");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblPastLeaves");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblOtherAddress");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblOtherAddress");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLocations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLocations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveRules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveRules");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                schema: "LMS",
                table: "tblLeaveRequests");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveRequests");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblLeaveCredit");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblLeaveCredit");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblHolidays");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblHolidays");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblEmployeeLeaves");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblEmployeeLeaves");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblEmergencyAddress");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblEmergencyAddress");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblCurrentAddress");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblCurrentAddress");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblConfiguration");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "LMS",
                table: "tblAttendance");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "LMS",
                table: "tblAttendance");
        }
    }
}

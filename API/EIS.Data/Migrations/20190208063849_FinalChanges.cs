using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EIS.Data.Migrations
{
    public partial class FinalChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LMS");

            migrationBuilder.CreateTable(
                name: "tblConfiguration",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidUpTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblConfiguration", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "tblRoles",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Access = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblHolidays",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Vacation = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblHolidays_tblLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "LMS",
                        principalTable: "tblLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblLeaveRules",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: false),
                    Validity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLeaveRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblLeaveRules_tblLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "LMS",
                        principalTable: "tblLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPerson",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    EmployeeCode = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    StreamId = table.Column<int>(type: "int", nullable: true),
                    PanCard = table.Column<string>(type: "varchar(10)", nullable: true),
                    AadharCard = table.Column<string>(type: "varchar(12)", nullable: true),
                    Image = table.Column<string>(type: "varchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "date", nullable: false),
                    LeavingDate = table.Column<DateTime>(type: "date", nullable: true),
                    MobileNumber = table.Column<string>(type: "varchar(10)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "getdate()"),
                    EmailAddress = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    Salary = table.Column<double>(type: "float", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    ReportingPersonId = table.Column<int>(nullable: false),
                    Gender = table.Column<string>(type: "varchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPerson_tblLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "LMS",
                        principalTable: "tblLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblPerson_tblRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "LMS",
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblAttendance",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    DateIn = table.Column<DateTime>(nullable: false),
                    TimeIn = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateOut = table.Column<DateTime>(type: "date", nullable: true),
                    TimeOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    TotalHours = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAttendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblAttendance_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblCurrentAddress",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PinCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCurrentAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCurrentAddress_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEmergencyAddress",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PinCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    Relation = table.Column<string>(type: "varchar(30)", nullable: false),
                    MobileNumber = table.Column<string>(type: "varchar(10)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmergencyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblEmergencyAddress_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEmployeeLeaves",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    LeaveType = table.Column<string>(nullable: true),
                    AllotedDays = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    TypeOfLeaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmployeeLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblEmployeeLeaves_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblEmployeeLeaves_tblLeaveRules_TypeOfLeaveId",
                        column: x => x.TypeOfLeaveId,
                        principalSchema: "LMS",
                        principalTable: "tblLeaveRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblLeaveCredit",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: false),
                    AllotedDays = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    LeaveId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLeaveCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblLeaveCredit_tblLeaveRules_LeaveId",
                        column: x => x.LeaveId,
                        principalSchema: "LMS",
                        principalTable: "tblLeaveRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblLeaveCredit_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblLeaveRequests",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    EmployeeName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false),
                    RequestedDays = table.Column<double>(type: "float", nullable: false),
                    ApprovedDays = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: true),
                    Reason = table.Column<string>(type: "varchar(200)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblLeaveRequests_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblLeaveRequests_tblLeaveRules_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "LMS",
                        principalTable: "tblLeaveRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblOtherAddress",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PinCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOtherAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblOtherAddress_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPastLeaves",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false),
                    RequestedDays = table.Column<double>(type: "float", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPastLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPastLeaves_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPermanentAddress",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PinCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPermanentAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPermanentAddress_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                schema: "LMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUsers_tblPerson_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "LMS",
                        principalTable: "tblPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblAttendance_PersonId",
                schema: "LMS",
                table: "tblAttendance",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCurrentAddress_PersonId",
                schema: "LMS",
                table: "tblCurrentAddress",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblEmergencyAddress_PersonId",
                schema: "LMS",
                table: "tblEmergencyAddress",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployeeLeaves_PersonId",
                schema: "LMS",
                table: "tblEmployeeLeaves",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblEmployeeLeaves_TypeOfLeaveId",
                schema: "LMS",
                table: "tblEmployeeLeaves",
                column: "TypeOfLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_tblHolidays_LocationId",
                schema: "LMS",
                table: "tblHolidays",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblLeaveCredit_LeaveId",
                schema: "LMS",
                table: "tblLeaveCredit",
                column: "LeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_tblLeaveCredit_PersonId",
                schema: "LMS",
                table: "tblLeaveCredit",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblLeaveRequests_PersonId",
                schema: "LMS",
                table: "tblLeaveRequests",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblLeaveRequests_TypeId",
                schema: "LMS",
                table: "tblLeaveRequests",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tblLeaveRules_LocationId",
                schema: "LMS",
                table: "tblLeaveRules",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblOtherAddress_PersonId",
                schema: "LMS",
                table: "tblOtherAddress",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPastLeaves_PersonId",
                schema: "LMS",
                table: "tblPastLeaves",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPermanentAddress_PersonId",
                schema: "LMS",
                table: "tblPermanentAddress",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_EmailAddress",
                schema: "LMS",
                table: "tblPerson",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_LocationId",
                schema: "LMS",
                table: "tblPerson",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_MobileNumber",
                schema: "LMS",
                table: "tblPerson",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_RoleId",
                schema: "LMS",
                table: "tblPerson",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPerson_TenantId_EmployeeCode",
                schema: "LMS",
                table: "tblPerson",
                columns: new[] { "TenantId", "EmployeeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblRoles_TenantId_Name",
                schema: "LMS",
                table: "tblRoles",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsers_PersonId",
                schema: "LMS",
                table: "tblUsers",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblUsers_TenantId_UserName",
                schema: "LMS",
                table: "tblUsers",
                columns: new[] { "TenantId", "UserName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAttendance",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblConfiguration",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblCurrentAddress",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblEmergencyAddress",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblEmployeeLeaves",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblHolidays",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblLeaveCredit",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblLeaveRequests",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblOtherAddress",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblPastLeaves",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblPermanentAddress",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblUsers",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblLeaveRules",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblPerson",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblLocations",
                schema: "LMS");

            migrationBuilder.DropTable(
                name: "tblRoles",
                schema: "LMS");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EIS.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Address");

            migrationBuilder.EnsureSchema(
                name: "Employee");

            migrationBuilder.EnsureSchema(
                name: "Leave");

            migrationBuilder.EnsureSchema(
                name: "Account");

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Access = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignationMaster",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Access = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignationMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveMaster",
                schema: "Leave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: false),
                    Days = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IdCard = table.Column<string>(type: "varchar(15)", nullable: false),
                    PanCard = table.Column<string>(type: "varchar(10)", nullable: true),
                    AadharCard = table.Column<string>(type: "varchar(12)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "date", nullable: false),
                    LeavingDate = table.Column<DateTime>(type: "date", nullable: false),
                    MobileNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    Salary = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DesignationId = table.Column<int>(nullable: false),
                    ReportingPersonId = table.Column<int>(nullable: false),
                    Gender = table.Column<string>(type: "varchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_DesignationMaster_DesignationId",
                        column: x => x.DesignationId,
                        principalSchema: "Employee",
                        principalTable: "DesignationMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrentAddress",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    table.PrimaryKey("PK_CurrentAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentAddress_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyAddress",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    MobileNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyAddress_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherAddress",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    table.PrimaryKey("PK_OtherAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherAddress_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermanentAddress",
                schema: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    table.PrimaryKey("PK_PermanentAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermanentAddress_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    DateIn = table.Column<DateTime>(type: "date", nullable: false),
                    TimeIn = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateOut = table.Column<DateTime>(type: "date", nullable: false),
                    TimeOut = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalHours = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaves",
                schema: "Leave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    TotalAlloted = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveCredit",
                schema: "Leave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    EmployeeName = table.Column<string>(type: "varchar(150)", nullable: false),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: false),
                    Days = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    LeaveId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveCredit_LeaveMaster_LeaveId",
                        column: x => x.LeaveId,
                        principalSchema: "Leave",
                        principalTable: "LeaveMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveCredit_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                schema: "Leave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    EmployeeName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LeaveType = table.Column<string>(type: "varchar(50)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false),
                    TotalRequestedDays = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: true),
                    Reason = table.Column<string>(type: "varchar(200)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Employee",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_LeaveMaster_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Leave",
                        principalTable: "LeaveMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Account",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Account",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Account",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "Account",
                table: "UserRoles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                schema: "Account",
                table: "Users",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrentAddress_PersonId",
                schema: "Address",
                table: "CurrentAddress",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyAddress_PersonId",
                schema: "Address",
                table: "EmergencyAddress",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherAddress_PersonId",
                schema: "Address",
                table: "OtherAddress",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PermanentAddress_PersonId",
                schema: "Address",
                table: "PermanentAddress",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_PersonId",
                schema: "Employee",
                table: "Attendance",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_DesignationId",
                schema: "Employee",
                table: "Person",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_PersonId",
                schema: "Leave",
                table: "EmployeeLeaves",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCredit_LeaveId",
                schema: "Leave",
                table: "LeaveCredit",
                column: "LeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCredit_PersonId",
                schema: "Leave",
                table: "LeaveCredit",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_PersonId",
                schema: "Leave",
                table: "LeaveRequests",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_TypeId",
                schema: "Leave",
                table: "LeaveRequests",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Account");

            migrationBuilder.DropTable(
                name: "CurrentAddress",
                schema: "Address");

            migrationBuilder.DropTable(
                name: "EmergencyAddress",
                schema: "Address");

            migrationBuilder.DropTable(
                name: "OtherAddress",
                schema: "Address");

            migrationBuilder.DropTable(
                name: "PermanentAddress",
                schema: "Address");

            migrationBuilder.DropTable(
                name: "Attendance",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeLeaves",
                schema: "Leave");

            migrationBuilder.DropTable(
                name: "LeaveCredit",
                schema: "Leave");

            migrationBuilder.DropTable(
                name: "LeaveRequests",
                schema: "Leave");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Account");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Account");

            migrationBuilder.DropTable(
                name: "LeaveMaster",
                schema: "Leave");

            migrationBuilder.DropTable(
                name: "Person",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "DesignationMaster",
                schema: "Employee");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "CreatedAt", "PetId", "PreferredDate", "Reason", "Status", "UserId" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 12, 1, 8, 25, 2, 473, DateTimeKind.Utc).AddTicks(1096), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 2, 8, 25, 2, 473, DateTimeKind.Utc).AddTicks(1075), "General Checkup", 0, new Guid("22222222-2222-2222-2222-222222222222") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}

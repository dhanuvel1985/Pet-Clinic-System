using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "PreferredDate" },
                values: new object[] { new DateTime(2025, 12, 3, 6, 46, 1, 372, DateTimeKind.Utc).AddTicks(6691), new DateTime(2025, 12, 4, 6, 46, 1, 372, DateTimeKind.Utc).AddTicks(6676) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "PreferredDate" },
                values: new object[] { new DateTime(2025, 12, 1, 8, 25, 2, 473, DateTimeKind.Utc).AddTicks(1096), new DateTime(2025, 12, 2, 8, 25, 2, 473, DateTimeKind.Utc).AddTicks(1075) });
        }
    }
}

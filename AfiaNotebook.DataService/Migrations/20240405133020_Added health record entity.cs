using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfiaNotebook.DataService.Migrations
{
    /// <inheritdoc />
    public partial class Addedhealthrecordentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthRecords_Users_UserId1",
                table: "HealthRecords");

            migrationBuilder.DropIndex(
                name: "IX_HealthRecords_UserId1",
                table: "HealthRecords");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "HealthRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "HealthRecords",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_UserId",
                table: "HealthRecords",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthRecords_Users_UserId",
                table: "HealthRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthRecords_Users_UserId",
                table: "HealthRecords");

            migrationBuilder.DropIndex(
                name: "IX_HealthRecords_UserId",
                table: "HealthRecords");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "HealthRecords",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "HealthRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_UserId1",
                table: "HealthRecords",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthRecords_Users_UserId1",
                table: "HealthRecords",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Time_Records.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordCreditTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "RecordCreditTime",
                table: "Records",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GoogleId",
                table: "AspNetUsers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordCreditTime",
                table: "Records");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleId",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

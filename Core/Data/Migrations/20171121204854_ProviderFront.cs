using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Core.Data.Migrations
{
    public partial class ProviderFront : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImapModel_id",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImapProviderModel",
                columns: table => new
                {
                    _id = table.Column<Guid>(nullable: false),
                    ImapHost = table.Column<string>(nullable: true),
                    ImapPort = table.Column<int>(nullable: false),
                    SmtpHost = table.Column<string>(nullable: true),
                    SmtpPort = table.Column<int>(nullable: false),
                    login = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    useSsl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImapProviderModel", x => x._id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ImapModel_id",
                table: "AspNetUsers",
                column: "ImapModel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ImapProviderModel_ImapModel_id",
                table: "AspNetUsers",
                column: "ImapModel_id",
                principalTable: "ImapProviderModel",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ImapProviderModel_ImapModel_id",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ImapProviderModel");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ImapModel_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImapModel_id",
                table: "AspNetUsers");
        }
    }
}

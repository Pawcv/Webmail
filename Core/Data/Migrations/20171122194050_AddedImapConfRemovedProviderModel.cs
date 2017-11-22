using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Core.Data.Migrations
{
    public partial class AddedImapConfRemovedProviderModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ImapConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    UseSsl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImapConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImapConfigurations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImapConfigurations_ApplicationUserId",
                table: "ImapConfigurations",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImapConfigurations");

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
    }
}

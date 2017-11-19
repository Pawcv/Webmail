using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Core.Data.Migrations
{
    public partial class provider_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "host",
                table: "ImapProviderModel");

            migrationBuilder.DropColumn(
                name: "port",
                table: "ImapProviderModel");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "ImapProviderModel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "ImapProviderModel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ImapHost",
                table: "ImapProviderModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImapPort",
                table: "ImapProviderModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SmtpHost",
                table: "ImapProviderModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmtpPort",
                table: "ImapProviderModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImapHost",
                table: "ImapProviderModel");

            migrationBuilder.DropColumn(
                name: "ImapPort",
                table: "ImapProviderModel");

            migrationBuilder.DropColumn(
                name: "SmtpHost",
                table: "ImapProviderModel");

            migrationBuilder.DropColumn(
                name: "SmtpPort",
                table: "ImapProviderModel");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "ImapProviderModel",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "ImapProviderModel",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "host",
                table: "ImapProviderModel",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "port",
                table: "ImapProviderModel",
                nullable: false,
                defaultValue: 0);
        }
    }
}

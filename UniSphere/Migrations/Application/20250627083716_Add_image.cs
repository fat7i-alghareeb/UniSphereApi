﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSphere.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class Add_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "system_controllers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "super_admins",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "student_credentials",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "professors",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "admins",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image",
                schema: "uni_sphere",
                table: "system_controllers");

            migrationBuilder.DropColumn(
                name: "image",
                schema: "uni_sphere",
                table: "super_admins");

            migrationBuilder.DropColumn(
                name: "image",
                schema: "uni_sphere",
                table: "admins");

            migrationBuilder.AlterColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "student_credentials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image",
                schema: "uni_sphere",
                table: "professors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}

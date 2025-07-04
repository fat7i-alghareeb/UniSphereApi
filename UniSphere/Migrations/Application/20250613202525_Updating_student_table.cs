﻿//<auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSphere.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class Updating_student_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subject_student_links_student_credentials_student_id_facult",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subject_student_links",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropIndex(
                name: "ix_subject_student_links_student_id_faculty_id",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropPrimaryKey(
                name: "pk_student_credentials",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.DropColumn(
                name: "faculty_id",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropColumn(
                name: "faculty_id",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.AddColumn<string>(
                name: "identity_id",
                schema: "uni_sphere",
                table: "student_credentials",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "student_number",
                schema: "uni_sphere",
                table: "student_credentials",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subject_student_links",
                schema: "uni_sphere",
                table: "subject_student_links",
                columns: new[] { "subject_id", "student_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_student_credentials",
                schema: "uni_sphere",
                table: "student_credentials",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_subject_student_links_student_id",
                schema: "uni_sphere",
                table: "subject_student_links",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_credentials_identity_id",
                schema: "uni_sphere",
                table: "student_credentials",
                column: "identity_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_subject_student_links_student_credentials_student_id",
                schema: "uni_sphere",
                table: "subject_student_links",
                column: "student_id",
                principalSchema: "uni_sphere",
                principalTable: "student_credentials",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subject_student_links_student_credentials_student_id",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subject_student_links",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropIndex(
                name: "ix_subject_student_links_student_id",
                schema: "uni_sphere",
                table: "subject_student_links");

            migrationBuilder.DropPrimaryKey(
                name: "pk_student_credentials",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.DropIndex(
                name: "ix_student_credentials_identity_id",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.DropColumn(
                name: "identity_id",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.DropColumn(
                name: "student_number",
                schema: "uni_sphere",
                table: "student_credentials");

            migrationBuilder.AddColumn<Guid>(
                name: "faculty_id",
                schema: "uni_sphere",
                table: "subject_student_links",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "faculty_id",
                schema: "uni_sphere",
                table: "student_credentials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_subject_student_links",
                schema: "uni_sphere",
                table: "subject_student_links",
                columns: new[] { "subject_id", "student_id", "faculty_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_student_credentials",
                schema: "uni_sphere",
                table: "student_credentials",
                columns: new[] { "id", "faculty_id" });

            migrationBuilder.CreateIndex(
                name: "ix_subject_student_links_student_id_faculty_id",
                schema: "uni_sphere",
                table: "subject_student_links",
                columns: new[] { "student_id", "faculty_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_subject_student_links_student_credentials_student_id_facult",
                schema: "uni_sphere",
                table: "subject_student_links",
                columns: new[] { "student_id", "faculty_id" },
                principalSchema: "uni_sphere",
                principalTable: "student_credentials",
                principalColumns: new[] { "id", "faculty_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}

﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSphere.Api.Migrations.Identity
{
    /// <inheritdoc />
    public partial class no_idea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "system_controller_id",
                schema: "identity",
                table: "asp_net_users",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "system_controller_id",
                schema: "identity",
                table: "asp_net_users");
        }
    }
}

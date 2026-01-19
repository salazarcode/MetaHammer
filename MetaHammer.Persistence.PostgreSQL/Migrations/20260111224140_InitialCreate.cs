/*
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaHammer.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    MetaProperties = table.Column<string>(type: "jsonb", nullable: false),
                    MetaRelations = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MetaTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Properties = table.Column<string>(type: "jsonb", nullable: false),
                    Relations = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaObjects_MetaTypes_MetaTypeId",
                        column: x => x.MetaTypeId,
                        principalTable: "MetaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaObjects_MetaTypeId",
                table: "MetaObjects",
                column: "MetaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaObjects_Properties",
                table: "MetaObjects",
                column: "Properties")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_MetaObjects_Relations",
                table: "MetaObjects",
                column: "Relations")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_MetaObjects_TenantId",
                table: "MetaObjects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaTypes_TenantId_Name",
                table: "MetaTypes",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaObjects");

            migrationBuilder.DropTable(
                name: "MetaTypes");
        }
    }
}
*/

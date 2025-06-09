using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.ContentModule.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuAndMenuItemsAddMenuAndMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentMenu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentMenuItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AssociatedObjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssociatedObjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssociatedObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    ParentMenuItemId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentMenuItem_ContentMenuItem_ParentMenuItemId",
                        column: x => x.ParentMenuItemId,
                        principalTable: "ContentMenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentMenuItem_ContentMenu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "ContentMenu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentMenuItem_MenuId",
                table: "ContentMenuItem",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentMenuItem_ParentMenuItemId",
                table: "ContentMenuItem",
                column: "ParentMenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentMenuItem");

            migrationBuilder.DropTable(
                name: "ContentMenu");
        }
    }
}

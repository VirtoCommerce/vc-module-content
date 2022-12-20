using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.ContentModule.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentMenuLinkList",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    OuterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenuLinkList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentMenuLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    AssociatedObjectType = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    AssociatedObjectName = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    AssociatedObjectId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MenuLinkListId = table.Column<string>(type: "character varying(128)", nullable: true),
                    OuterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenuLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentMenuLink_ContentMenuLinkList_MenuLinkListId",
                        column: x => x.MenuLinkListId,
                        principalTable: "ContentMenuLinkList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentMenuLink_MenuLinkListId",
                table: "ContentMenuLink",
                column: "MenuLinkListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentMenuLink");

            migrationBuilder.DropTable(
                name: "ContentMenuLinkList");
        }
    }
}

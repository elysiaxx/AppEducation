using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppEducation.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HOClasses",
                columns: table => new
                {
                    hocId = table.Column<string>(nullable: false),
                    ClassID = table.Column<string>(nullable: true),
                    _classClassID = table.Column<string>(nullable: true),
                    startTime = table.Column<DateTime>(nullable: false),
                    endTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOClasses", x => x.hocId);
                    table.ForeignKey(
                        name: "FK_HOClasses_Classes__classClassID",
                        column: x => x._classClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HOClasses__classClassID",
                table: "HOClasses",
                column: "_classClassID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HOClasses");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace AppEducation.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOClasses_Classes__classClassID",
                table: "HOClasses");

            migrationBuilder.DropIndex(
                name: "IX_HOClasses__classClassID",
                table: "HOClasses");

            migrationBuilder.DropColumn(
                name: "_classClassID",
                table: "HOClasses");

            migrationBuilder.AddColumn<string>(
                name: "LopHocClassID",
                table: "HOClasses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HOClasses_LopHocClassID",
                table: "HOClasses",
                column: "LopHocClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_HOClasses_Classes_LopHocClassID",
                table: "HOClasses",
                column: "LopHocClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOClasses_Classes_LopHocClassID",
                table: "HOClasses");

            migrationBuilder.DropIndex(
                name: "IX_HOClasses_LopHocClassID",
                table: "HOClasses");

            migrationBuilder.DropColumn(
                name: "LopHocClassID",
                table: "HOClasses");

            migrationBuilder.AddColumn<string>(
                name: "_classClassID",
                table: "HOClasses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HOClasses__classClassID",
                table: "HOClasses",
                column: "_classClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_HOClasses_Classes__classClassID",
                table: "HOClasses",
                column: "_classClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

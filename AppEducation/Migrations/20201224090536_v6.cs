using Microsoft.EntityFrameworkCore.Migrations;

namespace AppEducation.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOClasses_Classes_LopHocClassID",
                table: "HOClasses");

            migrationBuilder.DropIndex(
                name: "IX_HOClasses_LopHocClassID",
                table: "HOClasses");

            migrationBuilder.DropColumn(
                name: "ClassID",
                table: "HOClasses");

            migrationBuilder.DropColumn(
                name: "LopHocClassID",
                table: "HOClasses");

            migrationBuilder.RenameColumn(
                name: "hocId",
                table: "HOClasses",
                newName: "hocID");

            migrationBuilder.AddColumn<string>(
                name: "hocID",
                table: "Classes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_hocID",
                table: "Classes",
                column: "hocID");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_HOClasses_hocID",
                table: "Classes",
                column: "hocID",
                principalTable: "HOClasses",
                principalColumn: "hocID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_HOClasses_hocID",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_hocID",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "hocID",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "hocID",
                table: "HOClasses",
                newName: "hocId");

            migrationBuilder.AddColumn<string>(
                name: "ClassID",
                table: "HOClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LopHocClassID",
                table: "HOClasses",
                type: "nvarchar(450)",
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
    }
}

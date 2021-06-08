using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppEducation.Migrations
{
    public partial class v12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_HOClasses_hocID",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "HOClasses");

            migrationBuilder.DropIndex(
                name: "IX_Classes_hocID",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "OnlineStudent",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "hocID",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "RoomMemberID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    FileID = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileID);
                });

            migrationBuilder.CreateTable(
                name: "RoomDocuments",
                columns: table => new
                {
                    RoomDocumentID = table.Column<string>(nullable: false),
                    ClassInfoID = table.Column<string>(nullable: true),
                    ClassesClassID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomDocuments", x => x.RoomDocumentID);
                    table.ForeignKey(
                        name: "FK_RoomDocuments_Classes_ClassesClassID",
                        column: x => x.ClassesClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomMembers",
                columns: table => new
                {
                    RoomMemberID = table.Column<string>(nullable: false),
                    ClassID = table.Column<string>(nullable: true),
                    ClassesClassID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomMembers", x => x.RoomMemberID);
                    table.ForeignKey(
                        name: "FK_RoomMembers_Classes_ClassesClassID",
                        column: x => x.ClassesClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Describe = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    RoomDocumentID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_RoomDocuments_RoomDocumentID",
                        column: x => x.RoomDocumentID,
                        principalTable: "RoomDocuments",
                        principalColumn: "RoomDocumentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RoomMemberID",
                table: "AspNetUsers",
                column: "RoomMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RoomDocumentID",
                table: "Documents",
                column: "RoomDocumentID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDocuments_ClassesClassID",
                table: "RoomDocuments",
                column: "ClassesClassID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMembers_ClassesClassID",
                table: "RoomMembers",
                column: "ClassesClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_RoomMembers_RoomMemberID",
                table: "AspNetUsers",
                column: "RoomMemberID",
                principalTable: "RoomMembers",
                principalColumn: "RoomMemberID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RoomMembers_RoomMemberID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "RoomMembers");

            migrationBuilder.DropTable(
                name: "RoomDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RoomMemberID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoomMemberID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "OnlineStudent",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "hocID",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HOClasses",
                columns: table => new
                {
                    hocID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOClasses", x => x.hocID);
                });

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
    }
}

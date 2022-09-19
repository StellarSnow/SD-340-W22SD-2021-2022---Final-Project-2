using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.Migrations
{
    public partial class AddDeveloperToTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeveloperId",
                table: "Ticket",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_DeveloperId",
                table: "Ticket",
                column: "DeveloperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_DeveloperId",
                table: "Ticket",
                column: "DeveloperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_DeveloperId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_DeveloperId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "DeveloperId",
                table: "Ticket");
        }
    }
}

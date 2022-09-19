using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.Migrations
{
    public partial class RenameProjectsIdInTicketModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Project_ProjectsId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "Ticket",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_ProjectsId",
                table: "Ticket",
                newName: "IX_Ticket_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Project_ProjectId",
                table: "Ticket",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Project_ProjectId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Ticket",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_ProjectId",
                table: "Ticket",
                newName: "IX_Ticket_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Project_ProjectsId",
                table: "Ticket",
                column: "ProjectsId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

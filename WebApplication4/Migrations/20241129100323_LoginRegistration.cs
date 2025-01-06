using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication4.Migrations
{
    /// <inheritdoc />
    public partial class LoginRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministratorId",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministratorId1",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Administrator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrator", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AdministratorId",
                table: "Recipes",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AdministratorId1",
                table: "Recipes",
                column: "AdministratorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Administrator_AdministratorId",
                table: "Recipes",
                column: "AdministratorId",
                principalTable: "Administrator",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Administrator_AdministratorId1",
                table: "Recipes",
                column: "AdministratorId1",
                principalTable: "Administrator",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Administrator_AdministratorId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Administrator_AdministratorId1",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "Administrator");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_AdministratorId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_AdministratorId1",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "AdministratorId1",
                table: "Recipes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuinCalcData.Migrations
{
    public partial class AddBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BudgetId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Limit = table.Column<decimal>(nullable: false),
                    Warning = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTimeOffset>(nullable: true),
                    EndDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Income",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    HasArrived = table.Column<bool>(nullable: false),
                    DueTo = table.Column<DateTimeOffset>(nullable: true),
                    BudgetId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Income", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BudgetId",
                table: "Expenses",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Income_BudgetId",
                table: "Income",
                column: "BudgetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Budget_BudgetId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "Income");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BudgetId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Expenses");
        }
    }
}

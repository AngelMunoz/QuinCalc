using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuinCalcData.Migrations
{
  public partial class InitialCreate : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Expenses",
          columns: table => new
          {
            Id = table.Column<long>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Amount = table.Column<decimal>(type: "TEXT", nullable: false),
            DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Expenses", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Todos",
          columns: table => new
          {
            Id = table.Column<long>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Description = table.Column<string>(type: "TEXT", nullable: true),
            DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            IsDone = table.Column<bool>(type: "INTEGER", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Todos", x => x.Id);
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Expenses");

      migrationBuilder.DropTable(
          name: "Todos");
    }
  }
}

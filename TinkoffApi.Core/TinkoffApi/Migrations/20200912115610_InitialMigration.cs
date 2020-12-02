using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace TinkoffApi.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    eventType = table.Column<int>(nullable: true),
                    info = table.Column<string>(nullable: true),
                    eventDt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OperationsHistory",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    orderId = table.Column<string>(nullable: true),
                    orderType = table.Column<int>(nullable: false),
                    orderMode = table.Column<int>(nullable: false),
                    lots = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    time = table.Column<DateTime>(nullable: false),
                    figi = table.Column<string>(nullable: true),
                    operationStatus = table.Column<int>(nullable: false),
                    Commission_Currency = table.Column<int>(nullable: false),
                    Commission_Value = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StopLoss_Value = table.Column<double>(nullable: false),
                    StopLoss_Procent = table.Column<double>(nullable: false),
                    TakeProfit_Value = table.Column<double>(nullable: false),
                    TakeProfit_Procent = table.Column<double>(nullable: false),
                    Expire = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsHistory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioHistory",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    balance = table.Column<double>(nullable: false),
                    currency = table.Column<int>(nullable: false),
                    time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioHistory", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "OperationsHistory");

            migrationBuilder.DropTable(
                name: "PortfolioHistory");
        }
    }
}

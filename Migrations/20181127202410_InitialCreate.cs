using Microsoft.EntityFrameworkCore.Migrations;

namespace CasingDesign.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CasingInventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalDiameter = table.Column<double>(nullable: false),
                    InternalDiameter = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    YieldStrength = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Burst = table.Column<double>(nullable: false),
                    Collapse = table.Column<double>(nullable: false),
                    Axial = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasingInventory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasingInventory");
        }
    }
}

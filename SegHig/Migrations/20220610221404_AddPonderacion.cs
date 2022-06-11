using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class AddPonderacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ponderacion",
                table: "FormularioDetalles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ponderacion",
                table: "FormularioDetalles");
        }
    }
}

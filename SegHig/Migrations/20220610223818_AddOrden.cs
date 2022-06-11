using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class AddOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "TrabajoTipos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "Formularios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "FormularioDetalles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orden",
                table: "TrabajoTipos");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "Formularios");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "FormularioDetalles");
        }
    }
}

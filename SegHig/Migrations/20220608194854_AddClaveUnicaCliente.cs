using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class AddClaveUnicaCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Name_EmpresaId",
                table: "Clientes",
                columns: new[] { "Name", "EmpresaId" },
                unique: true,
                filter: "[EmpresaId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Name_EmpresaId",
                table: "Clientes");
        }
    }
}

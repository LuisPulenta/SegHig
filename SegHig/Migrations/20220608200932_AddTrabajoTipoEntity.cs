using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class AddTrabajoTipoEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrabajoTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrabajoTipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrabajoTipos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrabajoTipos_ClienteId",
                table: "TrabajoTipos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajoTipos_Name_ClienteId",
                table: "TrabajoTipos",
                columns: new[] { "Name", "ClienteId" },
                unique: true,
                filter: "[ClienteId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrabajoTipos");
        }
    }
}

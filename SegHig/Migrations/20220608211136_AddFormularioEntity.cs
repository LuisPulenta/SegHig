using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class AddFormularioEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "TrabajoTipos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrabajoTipoId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formularios_TrabajoTipos_TrabajoTipoId",
                        column: x => x.TrabajoTipoId,
                        principalTable: "TrabajoTipos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormularioDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FormularioId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormularioDetalles_Formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formularios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioDetalles_Description_FormularioId",
                table: "FormularioDetalles",
                columns: new[] { "Description", "FormularioId" },
                unique: true,
                filter: "[FormularioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioDetalles_FormularioId",
                table: "FormularioDetalles",
                column: "FormularioId");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_Name_TrabajoTipoId",
                table: "Formularios",
                columns: new[] { "Name", "TrabajoTipoId" },
                unique: true,
                filter: "[TrabajoTipoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_TrabajoTipoId",
                table: "Formularios",
                column: "TrabajoTipoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormularioDetalles");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "TrabajoTipos");
        }
    }
}

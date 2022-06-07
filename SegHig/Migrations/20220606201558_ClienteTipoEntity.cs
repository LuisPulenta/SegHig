using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class ClienteTipoEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClienteTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteTipos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClienteTipos_Name",
                table: "ClienteTipos",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteTipos");
        }
    }
}

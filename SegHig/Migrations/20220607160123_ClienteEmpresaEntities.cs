using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SegHig.Migrations
{
    public partial class ClienteEmpresaEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_ClienteTipos_ClienteTipoId",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_EmpresaTipos_EmpresaTipoId",
                table: "Empresa");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaTipos_Empresa_EmpresaId",
                table: "EmpresaTipos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empresa",
                table: "Empresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente");

            migrationBuilder.RenameTable(
                name: "Empresa",
                newName: "Empresas");

            migrationBuilder.RenameTable(
                name: "Cliente",
                newName: "Clientes");

            migrationBuilder.RenameIndex(
                name: "IX_Empresa_EmpresaTipoId",
                table: "Empresas",
                newName: "IX_Empresas_EmpresaTipoId");

            migrationBuilder.RenameIndex(
                name: "IX_Cliente_ClienteTipoId",
                table: "Clientes",
                newName: "IX_Clientes_ClienteTipoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_ClienteTipos_ClienteTipoId",
                table: "Clientes",
                column: "ClienteTipoId",
                principalTable: "ClienteTipos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_EmpresaTipos_EmpresaTipoId",
                table: "Empresas",
                column: "EmpresaTipoId",
                principalTable: "EmpresaTipos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaTipos_Empresas_EmpresaId",
                table: "EmpresaTipos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresas_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_ClienteTipos_ClienteTipoId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_EmpresaTipos_EmpresaTipoId",
                table: "Empresas");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaTipos_Empresas_EmpresaId",
                table: "EmpresaTipos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "Empresas",
                newName: "Empresa");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "Cliente");

            migrationBuilder.RenameIndex(
                name: "IX_Empresas_EmpresaTipoId",
                table: "Empresa",
                newName: "IX_Empresa_EmpresaTipoId");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_ClienteTipoId",
                table: "Cliente",
                newName: "IX_Cliente_ClienteTipoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empresa",
                table: "Empresa",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_ClienteTipos_ClienteTipoId",
                table: "Cliente",
                column: "ClienteTipoId",
                principalTable: "ClienteTipos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_EmpresaTipos_EmpresaTipoId",
                table: "Empresa",
                column: "EmpresaTipoId",
                principalTable: "EmpresaTipos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaTipos_Empresa_EmpresaId",
                table: "EmpresaTipos",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");
        }
    }
}

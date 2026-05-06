using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDigital.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_AGENCIAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_AGENCIAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRODUTOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TipoProduto = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    ValorMaximo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PrazoMaximoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Bandeira = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EmpresaConveniada = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    TaxaPortabilidade = table.Column<decimal>(type: "decimal(5,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRODUTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_CLIENTES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    AgenciaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Cnpj = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true),
                    RazaoSocial = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CLIENTES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CLIENTES_TB_AGENCIAS_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "TB_AGENCIAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONTRATACOES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProdutoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Observacao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ValorSolicitado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ScoreCredito = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TaxaMensalCalculada = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    EmpresaEmpregadora = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONTRATACOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACOES_TB_CLIENTES_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "TB_CLIENTES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACOES_TB_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TB_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TB_PRODUTOS",
                columns: new[] { "Id", "Nome", "PrazoMaximoMeses", "TipoProduto", "ValorMaximo" },
                values: new object[] { 1, "Empréstimo Pessoal", 60, "EMPRESTIMO", 50000m });

            migrationBuilder.InsertData(
                table: "TB_PRODUTOS",
                columns: new[] { "Id", "EmpresaConveniada", "Nome", "TaxaPortabilidade", "TipoProduto" },
                values: new object[] { 2, "FIAP", "Receber Salário", 0m, "RECEBER_SALARIO" });

            migrationBuilder.InsertData(
                table: "TB_PRODUTOS",
                columns: new[] { "Id", "Bandeira", "Nome", "TipoProduto" },
                values: new object[] { 3, "", "Maquininha de Cartão", "MAQUINA_CARTAO" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTES_AgenciaId",
                table: "TB_CLIENTES",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTES_Cnpj",
                table: "TB_CLIENTES",
                column: "Cnpj",
                unique: true,
                filter: "\"Cnpj\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTES_Cpf",
                table: "TB_CLIENTES",
                column: "Cpf",
                unique: true,
                filter: "\"Cpf\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACOES_ClienteId",
                table: "TB_CONTRATACOES",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACOES_ProdutoId",
                table: "TB_CONTRATACOES",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CONTRATACOES");

            migrationBuilder.DropTable(
                name: "TB_CLIENTES");

            migrationBuilder.DropTable(
                name: "TB_PRODUTOS");

            migrationBuilder.DropTable(
                name: "TB_AGENCIAS");
        }
    }
}

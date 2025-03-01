using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JessyProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "c2e");

            migrationBuilder.CreateTable(
                name: "Bouygues",
                schema: "c2e",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"Bouygues_Id_seq\"'::regclass)"),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Num_contrat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Vendeur = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Client = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Produite = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Equipe = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Bouygues_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Engie",
                schema: "c2e",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"Engie_Id_seq\"'::regclass)"),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Num_contrat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Vendeur = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Client = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Equipe = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Engie_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ohm",
                schema: "c2e",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"Ohm_Id_seq\"'::regclass)"),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Num_contrat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Vendeur = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Client = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Equipe = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Ohm_pkey", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "Bouygues_Num_contrat_key",
                schema: "c2e",
                table: "Bouygues",
                column: "Num_contrat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Engie_Num_contrat_key",
                schema: "c2e",
                table: "Engie",
                column: "Num_contrat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Ohm_Num_contrat_key",
                schema: "c2e",
                table: "Ohm",
                column: "Num_contrat",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bouygues",
                schema: "c2e");

            migrationBuilder.DropTable(
                name: "Engie",
                schema: "c2e");

            migrationBuilder.DropTable(
                name: "Ohm",
                schema: "c2e");
        }
    }
}

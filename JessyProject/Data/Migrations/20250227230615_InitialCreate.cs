using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Num_contrat = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    vendeur = table.Column<string>(type: "varchar(100)", fixedLength: true, maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "varchar(50)", fixedLength: true, maxLength: 50, nullable: false),
                    client = table.Column<string>(type: "varchar(255)", fixedLength: true, maxLength: 50, nullable: false),
                    produite = table.Column<string>(type: "varchar(50)", fixedLength: true, maxLength: 50, nullable: false),
                    equipe = table.Column<string>(type: "varchar(255)", fixedLength: true, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bouygues", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Engie",
                schema: "c2e",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Num_contrat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Vendeur = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Client = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Equipe = table.Column<string>(type: "character(255)", fixedLength: true, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__3214EC07F4414995", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ohm",
                schema: "c2e",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Num_contrat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Vendeur = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Client = table.Column<string>(type: "character(50)", fixedLength: true, maxLength: 50, nullable: true),
                    Equipe = table.Column<string>(type: "character(255)", fixedLength: true, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__3214EC07AAD91352", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bouygues_unique_contrat",
                schema: "c2e",
                table: "Bouygues",
                columns: new[] { "Num_contrat", "produite" },
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

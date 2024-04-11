using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharacterService.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Classes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Classes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Items",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BonusStrength = table.Column<int>(type: "int", nullable: false),
                BonusAgility = table.Column<int>(type: "int", nullable: false),
                BonusIntelligence = table.Column<int>(type: "int", nullable: false),
                BonusFaith = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Items", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Characters",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Health = table.Column<int>(type: "int", nullable: false),
                Mana = table.Column<int>(type: "int", nullable: false),
                BaseStrength = table.Column<int>(type: "int", nullable: false),
                BaseAgility = table.Column<int>(type: "int", nullable: false),
                BaseIntelligence = table.Column<int>(type: "int", nullable: false),
                BaseFaith = table.Column<int>(type: "int", nullable: false),
                ClassId = table.Column<int>(type: "int", nullable: false),
                CreatedById = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Characters", x => x.Id);
                table.ForeignKey(
                    name: "FK_Characters_Classes_ClassId",
                    column: x => x.ClassId,
                    principalTable: "Classes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CharacterItems",
            columns: table => new
            {
                CharactersId = table.Column<int>(type: "int", nullable: false),
                ItemsId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CharacterItems", x => new { x.CharactersId, x.ItemsId });
                table.ForeignKey(
                    name: "FK_CharacterItems_Characters_CharactersId",
                    column: x => x.CharactersId,
                    principalTable: "Characters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CharacterItems_Items_ItemsId",
                    column: x => x.ItemsId,
                    principalTable: "Items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterItems_ItemsId",
            table: "CharacterItems",
            column: "ItemsId");

        migrationBuilder.CreateIndex(
            name: "IX_Characters_ClassId",
            table: "Characters",
            column: "ClassId");

        migrationBuilder.CreateIndex(
            name: "IX_Characters_Name",
            table: "Characters",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Classes_Name",
            table: "Classes",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CharacterItems");

        migrationBuilder.DropTable(
            name: "Characters");

        migrationBuilder.DropTable(
            name: "Items");

        migrationBuilder.DropTable(
            name: "Classes");
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_1.Migrations
{
    public partial class AddAuthorAndRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // If the old 'Author' text column still exists, drop its default constraint (if any) and then drop the column
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Books','Author') IS NOT NULL
BEGIN
    DECLARE @dc sysname;
    SELECT @dc = d.name
    FROM sys.default_constraints d
    JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID(N'dbo.Books') AND c.name = N'Author';

    IF @dc IS NOT NULL
        EXEC(N'ALTER TABLE dbo.Books DROP CONSTRAINT [' + @dc + ']');

    ALTER TABLE dbo.Books DROP COLUMN [Author];
END
");

            // Add the new FK column (temporarily default to 0; we'll backfill it before adding the FK)
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Create Authors table
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            // Seed a default author and backfill existing rows in Books
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM dbo.Authors)
BEGIN
    INSERT INTO dbo.Authors([Name], [Email]) VALUES (N'Unknown Author', NULL);
END

DECLARE @aid INT = (SELECT TOP(1) [Id] FROM dbo.Authors ORDER BY [Id]);
UPDATE dbo.Books SET AuthorId = @aid WHERE AuthorId = 0 OR AuthorId IS NULL;
");

            // Index for the FK
            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            // Add FK AFTER backfill so it won't fail
            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // use Cascade if you prefer
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Books");

            // Recreate the old 'Author' text column (nullable to be safe)
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}

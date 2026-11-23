using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalDerick.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfessionalProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: true),
                    Photo = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalProfiles", x => x.Id);
                    table.CheckConstraint("CK_ProfessionalProfiles_Id", "Id = 1");
                });

            migrationBuilder.InsertData(
                table: "ProfessionalProfiles",
                columns: new[] { "Id", "Name", "Title", "Bio", "Photo", "Country", "State" },
                values: new object[] { 1, "Nome do Dev", "Dev. de Software", null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalProfiles");
        }
    }
}


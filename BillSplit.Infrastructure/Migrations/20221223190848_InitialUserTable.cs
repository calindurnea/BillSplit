using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BillSplit.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialUserTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "billsplit");

        migrationBuilder.CreateTable(
            name: "User",
            schema: "billsplit",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                PhoneNumber = table.Column<long>(type: "bigint", nullable: false),
                Password = table.Column<string>(type: "text", nullable: false),
                IsSuperUser = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "User",
            schema: "billsplit");
    }
}

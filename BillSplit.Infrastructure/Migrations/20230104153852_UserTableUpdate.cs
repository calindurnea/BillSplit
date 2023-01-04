using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillSplit.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserTableUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "UpdatedDate",
            schema: "billsplit",
            table: "User",
            type: "timestamp with time zone",
            nullable: true,
            defaultValueSql: "CURRENT_TIMESTAMP",
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldDefaultValueSql: "CURRENT_TIMESTAMP");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "UpdatedDate",
            schema: "billsplit",
            table: "User",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP",
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true,
            oldDefaultValueSql: "CURRENT_TIMESTAMP");
    }
}

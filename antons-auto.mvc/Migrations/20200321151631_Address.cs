using Microsoft.EntityFrameworkCore.Migrations;

namespace antons_auto.mvc.Migrations
{
    public partial class Address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Car",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressNo",
                table: "Car",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Car",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Car",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "Car",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "AddressNo",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Car");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Scripters.Regula.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryTrackingDistributorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.AddColumn<string>(
                name: "cargo",
                table: "deliveries",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "destination",
                table: "deliveries",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "inventories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    inventory_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_inventories", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gas_cylinder_stocks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cylinder_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    available = table.Column<int>(type: "int", nullable: false),
                    in_transit = table.Column<int>(type: "int", nullable: false),
                    observed = table.Column<int>(type: "int", nullable: false),
                    out_of_service = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    inventory_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_gas_cylinder_stocks", x => x.id);
                    table.ForeignKey(
                        name: "f_k_gas_cylinder_stocks_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    movement_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    cylinder_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    provider_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    inventory_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_movements", x => x.id);
                    table.ForeignKey(
                        name: "f_k_movements_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "company_movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    destination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    movement_reason = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    observation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_company_movements_movements_id",
                        column: x => x.id,
                        principalTable: "movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "distributor_movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    outbound_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_distributor_movements_movements_id",
                        column: x => x.id,
                        principalTable: "movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "cargo", "destination", "item_count", "scheduled_time", "status" },
                values: new object[] { "3 balones", "Entrega Centro", 3, new DateTime(2026, 6, 16, 14, 25, 0, 0, DateTimeKind.Unspecified), "ONROUTE" });

            migrationBuilder.UpdateData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Juan López");

            migrationBuilder.InsertData(
                table: "delivery_responsibles",
                columns: new[] { "id", "created_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 2, null, "Pedro Salas", null },
                    { 3, null, "Ana Gómez", null },
                    { 4, null, "Carlos Ruiz", null },
                    { 5, null, "Luis Torres", null },
                    { 6, null, "Raúl Méndez", null }
                });

            migrationBuilder.UpdateData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "brand", "plate", "type" },
                values: new object[] { "Honda", "A38-210", "Moto" });

            migrationBuilder.InsertData(
                table: "delivery_vehicles",
                columns: new[] { "id", "brand", "created_at", "plate", "type", "updated_at" },
                values: new object[,]
                {
                    { 2, "Toyota", null, "C5R-982", "Camioneta", null },
                    { 3, "Yamaha", null, "B12-400", "Moto", null },
                    { 4, "Hino", null, "XYZ-787", "Camión", null },
                    { 5, "Honda", null, "D45-001", "Moto", null },
                    { 6, "Bajaj", null, "X1W-445", "Moto", null }
                });

            migrationBuilder.InsertData(
                table: "driver_locations",
                columns: new[] { "id", "created_at", "delivery_id", "driver_id", "eta", "last_updated", "latitude", "longitude", "updated_at" },
                values: new object[] { 1, null, 1, 101, null, new DateTime(2026, 7, 2, 7, 26, 6, 837, DateTimeKind.Utc).AddTicks(5065), -12.0464, -77.0428, null });

            migrationBuilder.InsertData(
                table: "deliveries",
                columns: new[] { "id", "cargo", "created_at", "delivered_at", "destination", "driver_id", "item_count", "responsible_id", "scheduled_time", "status", "updated_at", "vehicle_id" },
                values: new object[,]
                {
                    { 2, "2 balones", null, "13:45", "Av. Los Pinos 456", 102, 2, 2, new DateTime(2026, 6, 16, 13, 45, 0, 0, DateTimeKind.Unspecified), "DELIVERED", null, 2 },
                    { 3, "1 balón", null, "12:05", "Jr. Las Flores 123", 103, 1, 3, new DateTime(2026, 6, 16, 12, 5, 0, 0, DateTimeKind.Unspecified), "DELIVERED", null, 3 },
                    { 4, "10 balones de 45 kg", null, null, "Restaurante El Mar", 104, 10, 4, new DateTime(2026, 6, 16, 14, 50, 0, 0, DateTimeKind.Unspecified), "ONROUTE", null, 4 },
                    { 5, "5 balones", null, "11:15", "Calle Lima 88", 105, 5, 5, new DateTime(2026, 6, 16, 11, 15, 0, 0, DateTimeKind.Unspecified), "DELIVERED", null, 5 },
                    { 6, "1 balón", null, null, "Av. Grau 200", 106, 1, 6, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Unspecified), "NOTDELIVERED", null, 6 }
                });

            migrationBuilder.InsertData(
                table: "driver_locations",
                columns: new[] { "id", "created_at", "delivery_id", "driver_id", "eta", "last_updated", "latitude", "longitude", "updated_at" },
                values: new object[,]
                {
                    { 2, null, 2, 102, null, new DateTime(2026, 7, 2, 7, 26, 6, 837, DateTimeKind.Utc).AddTicks(5756), -12.06, -77.037499999999994, null },
                    { 3, null, 4, 104, null, new DateTime(2026, 7, 2, 7, 26, 6, 837, DateTimeKind.Utc).AddTicks(5757), -12.055, -77.040000000000006, null }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_gas_cylinder_stocks_inventory_id",
                table: "gas_cylinder_stocks",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "i_x_inventories_owner_profile_id_inventory_type",
                table: "inventories",
                columns: new[] { "owner_profile_id", "inventory_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_movements_inventory_id",
                table: "movements",
                column: "inventory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company_movements");

            migrationBuilder.DropTable(
                name: "distributor_movements");

            migrationBuilder.DropTable(
                name: "gas_cylinder_stocks");

            migrationBuilder.DropTable(
                name: "movements");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DeleteData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "driver_locations",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "driver_locations",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "driver_locations",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "cargo",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "destination",
                table: "deliveries");

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    criticality = table.Column<string>(type: "longtext", nullable: false),
                    detected_at = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ppm_level = table.Column<double>(type: "double", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    zone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "item_count", "scheduled_time", "status" },
                values: new object[] { 5, new DateTime(2026, 6, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), "PENDING" });

            migrationBuilder.UpdateData(
                table: "delivery_responsibles",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Responsable de prueba");

            migrationBuilder.UpdateData(
                table: "delivery_vehicles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "brand", "plate", "type" },
                values: new object[] { "Toyota", "ABC-123", "Van" });
        }
    }
}

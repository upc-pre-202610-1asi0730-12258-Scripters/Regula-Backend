using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Scripters.Regula.Platform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "commercial_customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    active_debt_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    debt_count = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_commercial_customers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "delivery_responsibles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_delivery_responsibles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "delivery_vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    plate = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    brand = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_delivery_vehicles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    stripe_customer_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    stripe_subscription_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    current_period_end = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false),
                    password_hash = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "commercial_daily_sales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    transaction_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    cylinder_type_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    cylinder_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    payment_type = table.Column<string>(type: "longtext", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    customer_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    distributor_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_commercial_daily_sales", x => x.id);
                    table.ForeignKey(
                        name: "f_k_commercial_daily_sales_commercial_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "commercial_customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "commercial_debts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    remaining_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_commercial_debts", x => x.id);
                    table.ForeignKey(
                        name: "f_k_commercial_debts_commercial_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "commercial_customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "deliveries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    driver_id = table.Column<int>(type: "int", nullable: false),
                    responsible_id = table.Column<int>(type: "int", nullable: false),
                    vehicle_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    item_count = table.Column<int>(type: "int", nullable: false),
                    cargo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    destination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    scheduled_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    delivered_at = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_deliveries", x => x.id);
                    table.ForeignKey(
                        name: "f_k_deliveries_delivery_responsibles_responsible_id",
                        column: x => x.responsible_id,
                        principalTable: "delivery_responsibles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_deliveries_delivery_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "delivery_vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "commercial_debt_payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    customer_debt_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    previous_remaining_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    new_remaining_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    note = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_commercial_debt_payments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_commercial_debt_payments_commercial_debts_customer_debt_id",
                        column: x => x.customer_debt_id,
                        principalTable: "commercial_debts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "driver_locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    delivery_id = table.Column<int>(type: "int", nullable: false),
                    driver_id = table.Column<int>(type: "int", nullable: false),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    eta = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_driver_locations", x => x.id);
                    table.ForeignKey(
                        name: "f_k_driver_locations_deliveries_delivery_id",
                        column: x => x.delivery_id,
                        principalTable: "deliveries",
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

            migrationBuilder.InsertData(
                table: "commercial_customers",
                columns: new[] { "id", "active_debt_amount", "created_at", "debt_count", "name", "updated_at" },
                values: new object[] { 1, 0m, null, 0, "Cliente de prueba", null });

            migrationBuilder.InsertData(
                table: "delivery_responsibles",
                columns: new[] { "id", "created_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, null, "Juan López", null },
                    { 2, null, "Pedro Salas", null },
                    { 3, null, "Ana Gómez", null },
                    { 4, null, "Carlos Ruiz", null },
                    { 5, null, "Luis Torres", null },
                    { 6, null, "Raúl Méndez", null }
                });

            migrationBuilder.InsertData(
                table: "delivery_vehicles",
                columns: new[] { "id", "brand", "created_at", "plate", "type", "updated_at" },
                values: new object[,]
                {
                    { 1, "Honda", null, "A38-210", "Moto", null },
                    { 2, "Toyota", null, "C5R-982", "Camioneta", null },
                    { 3, "Yamaha", null, "B12-400", "Moto", null },
                    { 4, "Hino", null, "XYZ-787", "Camión", null },
                    { 5, "Honda", null, "D45-001", "Moto", null },
                    { 6, "Bajaj", null, "X1W-445", "Moto", null }
                });

            migrationBuilder.InsertData(
                table: "deliveries",
                columns: new[] { "id", "cargo", "created_at", "delivered_at", "destination", "driver_id", "item_count", "responsible_id", "scheduled_time", "status", "updated_at", "vehicle_id" },
                values: new object[,]
                {
                    { 1, "3 balones", null, null, "Entrega Centro", 101, 3, 1, new DateTime(2026, 6, 16, 14, 25, 0, 0, DateTimeKind.Unspecified), "ONROUTE", null, 1 },
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
                    { 1, null, 1, 101, null, new DateTime(2026, 7, 5, 16, 48, 35, 456, DateTimeKind.Utc).AddTicks(5989), -12.0464, -77.0428, null },
                    { 2, null, 2, 102, null, new DateTime(2026, 7, 5, 16, 48, 35, 456, DateTimeKind.Utc).AddTicks(6866), -12.06, -77.037499999999994, null },
                    { 3, null, 4, 104, null, new DateTime(2026, 7, 5, 16, 48, 35, 456, DateTimeKind.Utc).AddTicks(6868), -12.055, -77.040000000000006, null }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_commercial_daily_sales_customer_id",
                table: "commercial_daily_sales",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_commercial_debt_payments_customer_debt_id",
                table: "commercial_debt_payments",
                column: "customer_debt_id");

            migrationBuilder.CreateIndex(
                name: "i_x_commercial_debts_customer_id",
                table: "commercial_debts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_deliveries_responsible_id",
                table: "deliveries",
                column: "responsible_id");

            migrationBuilder.CreateIndex(
                name: "i_x_deliveries_vehicle_id",
                table: "deliveries",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "i_x_driver_locations_delivery_id",
                table: "driver_locations",
                column: "delivery_id");

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

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commercial_daily_sales");

            migrationBuilder.DropTable(
                name: "commercial_debt_payments");

            migrationBuilder.DropTable(
                name: "company_movements");

            migrationBuilder.DropTable(
                name: "distributor_movements");

            migrationBuilder.DropTable(
                name: "driver_locations");

            migrationBuilder.DropTable(
                name: "gas_cylinder_stocks");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "commercial_debts");

            migrationBuilder.DropTable(
                name: "movements");

            migrationBuilder.DropTable(
                name: "deliveries");

            migrationBuilder.DropTable(
                name: "commercial_customers");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DropTable(
                name: "delivery_responsibles");

            migrationBuilder.DropTable(
                name: "delivery_vehicles");
        }
    }
}

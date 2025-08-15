using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriverBooking.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyrelationshipmmVehicleStageFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_StageFees_StageFeeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_StageFeeId",
                table: "Vehicles");

            migrationBuilder.CreateTable(
                name: "StageFeeVehicle",
                columns: table => new
                {
                    StageFeesId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageFeeVehicle", x => new { x.StageFeesId, x.VehicleId });
                    table.ForeignKey(
                        name: "FK_StageFeeVehicle_StageFees_StageFeesId",
                        column: x => x.StageFeesId,
                        principalTable: "StageFees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageFeeVehicle_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StageFeeVehicle_VehicleId",
                table: "StageFeeVehicle",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageFeeVehicle");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StageFeeId",
                table: "Vehicles",
                column: "StageFeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_StageFees_StageFeeId",
                table: "Vehicles",
                column: "StageFeeId",
                principalTable: "StageFees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

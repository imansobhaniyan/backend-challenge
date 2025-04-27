using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ighan.FlightSchedule.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginCityId_DestinationCityId",
                table: "Routes",
                columns: new[] { "OriginCityId", "DestinationCityId" })
                .Annotation("SqlServer:Include", new[] { "RouteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureTime",
                table: "Flights",
                column: "DepartureTime")
                .Annotation("SqlServer:Include", new[] { "RouteId", "ArrivalTime", "AirlineId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginCityId_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DepartureTime",
                table: "Flights");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ighan.FlightSchedule.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flights_RouteId",
                table: "Flights");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AgencyId",
                table: "Subscriptions",
                column: "AgencyId")
                .Annotation("SqlServer:Include", new[] { "DestinationCityId", "OriginCityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DepartureDate_OriginCityId_DestinationCityId",
                table: "Routes",
                columns: new[] { "DepartureDate", "OriginCityId", "DestinationCityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId")
                .Annotation("SqlServer:Include", new[] { "DepartureTime", "ArrivalTime", "AirlineId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_AgencyId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DepartureDate_OriginCityId_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Flights_RouteId",
                table: "Flights");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");
        }
    }
}

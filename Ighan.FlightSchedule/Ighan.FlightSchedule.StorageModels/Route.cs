namespace Ighan.FlightSchedule.StorageModels;

public sealed class Route
{
    public int RouteId { get; set; }
    public int OriginCityId { get; set; }
    public int DestinationCityId { get; set; }

    public DateTime DepartureDate { get; set; }

    public List<Flight> Flights { get; set; } = [];
}
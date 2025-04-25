namespace Ighan.FlightSchedule.StorageModels;

public sealed class Subscription
{
    public int SubscriptionId { get; set; }
    public int AgencyId { get; set; }
    public int OriginCityId { get; set; }
    public int DestinationCityId { get; set; }
}
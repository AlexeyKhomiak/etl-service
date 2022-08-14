namespace etl_service.models
{
    internal class CityData
    {
        public string City { get; set; } = string.Empty;
        public List<Service> services { get; set; } = new List<Service>();
        public decimal Total { get; set; } = 0;
    }
}

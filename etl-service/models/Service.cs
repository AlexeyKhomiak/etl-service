namespace etl_service.models
{
    internal class Service
    {
        public string Name { get; set; } = string.Empty;
        public List<Payer> payers { get; set; } = new List<Payer>();
        public decimal Total { get; set; } = 0;
    }
}

namespace etl_service.models
{
    public class Line
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public decimal Payment { get; set; }
        public DateOnly Date { get; set; } 
        public long Acount_Number { get; set; }
        public string Service { get; set; }

    }
}

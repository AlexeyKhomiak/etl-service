using etl_service.services;
using System.Text.Json.Serialization;

namespace etl_service.models
{
    internal class Payer
    {
        public string Name { get; set; } = string.Empty;
        public decimal Payment { get; set; } = decimal.Zero;
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Date { get; set; }
        public long Account_Number { get; set; } = 0;

    }
}

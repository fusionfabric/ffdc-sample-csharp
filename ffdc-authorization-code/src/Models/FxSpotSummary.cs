namespace ffdc_authorization_code.Models
{
    public class FxSpotSummary
    {
        public string id { get; set; }
        public string currencyPair { get; set; }
        public string direction { get; set; }
        public MoneyAmount domesticAmount { get; set; }
        public MoneyAmount foreignAmount { get; set; }
        public decimal exchangeRate { get; set; }
        public string legalEntityId { get; set; }
        public string counterpartyId { get; set; }
        public string tradeDate { get; set; }
        public string settlementDate { get; set; }
        public string traderId { get; set; }
    }
}

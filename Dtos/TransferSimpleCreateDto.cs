namespace SCustomers.Dtos
{
    public class TransferSimpleCreateDto
    {
        public decimal? Amount { get; set; }
        public string Phone { get; set; }
        public int? CurrencyId { get; set; }
        public int TransferTypeId { get; set; }
        public string Note { get; set; }
    }
}

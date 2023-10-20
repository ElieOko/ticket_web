namespace SCustomers.Dtos
{
    public class TransferSimpleUpdateDto
    {
        public int TransferId { get; set; }
        public decimal? Amount { get; set; }
        public string Phone { get; set; }
        public int? CurrencyId { get; set; }
        public int TransferTypeId { get; set; }
        public string Note { get; set; }
    }
}

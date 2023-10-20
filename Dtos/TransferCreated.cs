using System;

namespace SCustomers.Dtos
{
    public class TransferCreated
    {
        public int TransferId { get; set; }
        public int Id { get { return TransferId; } }
        public int TransferTypeId { get; set; }
        public string TransferTypeName { get; set; }
        public int? FromBranchId { get; set; }
        public string FromBranchName { get; set; }
        public int? ToBranchId { get; set; }
        public string ToBranchName { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public int OrderNumber { get; set; }
        public string Barcode { get; set; }
        public int? IntervalId { get; set; }
        public int? CardId { get; set; }
        public string CardName { get; set; }
        public DateTime? CardExpiryDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string DateFormated { get => DateCreated.ToString("yyyyMMdd HH:mm:ss"); }
    }
}

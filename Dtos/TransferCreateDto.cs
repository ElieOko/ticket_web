using SCustomers.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCustomers.Dtos
{
    public class TransferCreateDto
    {
        public int TransferTypeId { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyId { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public Branch Branch { get; set; }
        public int? CardId { get; set; }
        public int? IntervalId { get; set; }
        public string Signature { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CardExpiryDate { get; set; }
        public string UniqueString { get; set; }
    }
}

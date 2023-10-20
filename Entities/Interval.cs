using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TIntervals")]
    public class Interval
    {
        public int IntervalId { get; set; }
        [Column("TransferTypeFId")]
        public int? TransferTypeId { get; set; }
        [Column("CurrencyFId")]
        public int? CurrencyId { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public TransferType TransferType { get; set; }
    }
}

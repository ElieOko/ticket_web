using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TOrderNumbers")]
    public class OrderNumber
    {
        public int OrderNumberId { get; set; }
        public int Number { get; set; }
        [Column("TransferTypeFId")]
        public int TransferTypeId { get; set; }
        [Column("BranchFId")]
        public int BranchId { get; set; }
        public string CreatedDate { get; set; }
    }
}

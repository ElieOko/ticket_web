using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TTransferTypes")]
    public class TransferType
    {
        public int TransferTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}

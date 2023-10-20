using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TTransferStatuses")]
    public class TransferStatus
    {
        public int TransferStatusId { get; set; }
        public string Name { get; set; }
    }
}

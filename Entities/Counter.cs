using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TCounters")]
    public class Counter
    {
        public int CounterId { get; set; }
        public string Name { get; set; }
        [Column("BranchFId")]
        public int? BranchId { get; set; }
    }
}

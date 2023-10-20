using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TCalls")]
    public class Call
    {
        public int CallId { get; set; }
        public int? Ticket { get; set; }
        [Column("CounterFId")]
        public int? CounterId { get; set; }
        public string Note { get; set; }
        [Column("UserFId")]
        public int? UserId { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}

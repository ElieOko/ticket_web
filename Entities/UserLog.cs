using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TUserLogs")]
    public class UserLog
    {
        public int UserLogId { get; set; }
        [Column("UserFId")]
        public int UserId { get; set; }
        public string ClientType { get; set; }
        public string ClientIpAddress { get; set; }
        public string AccessType { get; set; }
        public DateTime LogTime { get; set; }
    }
}

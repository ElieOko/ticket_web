using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TUsers")]
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public byte AccessLevel { get; set; }
        public bool Locked { get; set; }
        public int AccessFailedCount { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public int MaxAttempt { get; set; }
        public DateTime DateCreated { get; set; }
        [Column("BranchFId")]
        public int BranchId { get; set; }
        public string UserType { get; set; }
        public Branch Branch { get; set; }
        public List<UserPermission> UserPermissions { get; set; }
    }
}

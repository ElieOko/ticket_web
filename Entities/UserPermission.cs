using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TUserPermissions")]
    public class UserPermission
    {
        public int UserPermissionId { get; set; }
        [Column("UserFId")]
        public int UserId { get; set; }
        [Column("TransferTypeFId")]
        public int TransferTypeId { get; set; }
        public TransferType TransferType { get; set; }
    }
}

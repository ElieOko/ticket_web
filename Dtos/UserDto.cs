using SCustomers.Entities;
using SCustomers.Models;
using System.Collections.Generic;

namespace SCustomers.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public byte AccessLevel { get; set; }
        public bool Locked { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public List<int> Permissions { get; set; }
        public string Type
        {
            get
            {
                return AccessLevel switch
                {
                    Acl.User => "Utilisateur",
                    Acl.Manager => "Chef d'agence",
                    Acl.Admin => "Administrateur",
                    _ => "Unknown Type",
                };
            }
        }
    }
}

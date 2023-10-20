using SCustomers.Entities;

namespace SCustomers.Models
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public int BranchId { get; set; }
        public string UserType { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanManage { get; set; }
        public Branch Branch { get; set; }
    }
}

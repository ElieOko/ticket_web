using SCustomers.Entities;

namespace SCustomers.Dtos
{
    public class UserGridUpdateDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public byte AccessLevel { get; set; }
        public Branch Branch { get; set; }
        public bool Locked { get; set; }
    }
}

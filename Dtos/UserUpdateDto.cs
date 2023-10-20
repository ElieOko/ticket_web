using System.Collections.Generic;

namespace SCustomers.Dtos
{
    public class UserUpdateDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public byte AccessLevel { get; set; }
        public int Branch { get; set; }
        public List<int> Permissions { get; set; }
    }
}

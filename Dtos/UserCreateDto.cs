using SCustomers.Entities;
using System.Collections.Generic;

namespace SCustomers.Dtos
{
    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int MaxAttempt { get; set; } = 3;
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public byte AccessLevel { get; set; }
        public int Branch { get; set; }
        public bool Locked { get; set; } = false;
        public List<int> Permissions { get; set; }
    }
}

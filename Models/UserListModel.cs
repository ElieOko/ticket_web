using SCustomers.Dtos;
using System.Collections.Generic;

namespace SCustomers.Models
{
    public class UserListModel
    {
        public List<UserDto> Users { get; set; } = new List<UserDto>();
    }
}

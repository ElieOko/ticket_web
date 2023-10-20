namespace SCustomers.Dtos
{
    public class UserManageDto
    {
        public int UserId { get; set; }
        public string Branch { get; set; }
        public UserProfileDto UserProfile { get; set; }
    }
}

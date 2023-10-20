namespace SCustomers.Dtos
{
    public class CheckUsernameDto
    {
        public class Request
        {
            public string Username { get; set; }
        }

        public class Response
        {
            public bool IsTaken { get; set; }
        }
    }
}

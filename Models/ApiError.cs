namespace SCustomers.Models
{
    public class ApiError
    {
        public ApiError(string message, int errorCode)
        {
            Message = message;
            ErrorCode = errorCode;
        }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}

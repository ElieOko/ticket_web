using System;

namespace SCustomers.Exceptions
{
    public class UnauthorizedAccessException:Exception
    {
        public UnauthorizedAccessException(string message, int errorCode)
            :base(message)
        {
            ErrorCode = errorCode;
        }
        public int ErrorCode { get; set; }
    }
}

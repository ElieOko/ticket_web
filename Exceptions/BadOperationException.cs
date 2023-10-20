using System;

namespace SCustomers.Exceptions
{
    public class BadOperationException:Exception
    {
        public BadOperationException(string message)
            :base(message)
        {

        }
        public int ErrorCode { get; set; }
    }
}

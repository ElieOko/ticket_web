using SCustomers.Entities;

namespace SCustomers.Dtos
{
    public class TransferCallDto
    {
        public int OrderNumber { get; set; }
        public Counter Counter { get; set; }
        public string Note { get; set; }
        public bool ForPayment { get; set; }
    }
}

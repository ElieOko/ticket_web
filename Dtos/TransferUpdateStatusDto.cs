namespace SCustomers.Dtos
{
    public class TransferUpdateStatusDto
    {
        public int OrderNumber { get; set; }
        public int TransferStatusId { get; set; }
        public string CompleteNote { get; set; }
        public bool CompleteAndPay { get; set; }
    }
}

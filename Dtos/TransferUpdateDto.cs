using SCustomers.Entities;

namespace SCustomers.Dtos
{
    public class TransferUpdateDto
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public string CompleteNote { get; set; }
    }
}

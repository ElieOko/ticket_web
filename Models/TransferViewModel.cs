using SCustomers.Entities;

namespace SCustomers.Models
{
    public class TransferViewModel
    {
        public int TransferId { get; set; }
        public int OrderNumber { get; set; }
        public double? Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CreatedTime { get; set; }
        public string ProcessedTime { get; set; }
        public int ProcessDuration { get; set; }
        public string PaidTime { get; set; }
        public int PayDuration { get; set; }
        public string CompleteNote { get; set; }
        public Currency Currency { get; set; }
        public TransferType TransferType { get; set; }
        public int? TransferStatusId { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public string Branch { get; set; }
        public bool Completed { get; set; }
        public string CompleteUser { get; set; }
        public string PaidUser { get; set; }
    }
}

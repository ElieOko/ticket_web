using SCustomers.Entities;

namespace SCustomers.Dtos
{
    public class TransferDto
    {
        public int TransferId { get; set; }
        public int OrderNumber { get; set; }
        public int TransferTypeId { get; set; }
        public string TransferType { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string DateCreated { get; set; }
        public string FinS { get; set; }
        public int DureeS { get; set; }
        public string FinP { get; set; }
        public int DureeP { get; set; }
        public string CompleteNote { get; set; }
        public int? TransferStatusId { get; set; }
        public string ImagePath { get; set; }
        public string Signature { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public string Branch { get; set; }
        public bool Completed { get; set; }
        public string UserS { get; set; }
        public string UserP { get; set; }
    }
}

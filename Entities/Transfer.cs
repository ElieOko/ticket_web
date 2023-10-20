using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TTransfers")]
    public class Transfer
    {
        public int TransferId { get; set; }
        [Column("TransferTypeFId")]
        public int TransferTypeId { get; set; }
        public int? FromBranchId { get; set; }
        public int? ToBranchId { get; set; }
        public decimal? Amount { get; set; }
        [Column("CurrencyFId")]
        public int? CurrencyId { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public string ImagePath { get; set; }
        [Column("CardFId")]
        public int? CardId { get; set; }
        [Column("TransferStatusFId")]
        public int? TransferStatusId { get; set; }
        public bool? Completed { get; set; }
        public string CompleteNote { get; set; }
        public DateTime? DateCompleted { get; set; }
        [Column("CompleteUserFId")]
        public int? CompleteUserId { get; set; }
        public int OrderNumber { get; set; }
        public string Barcode { get; set; }
        public DateTime? CardExpiryDate { get; set; }
        public string TokenPath { get; set; }
        [Column("IntervalFId")]
        public int? IntervalId { get; set; }
        public string Signature { get; set; }
        public DateTime? TimeCalled { get; set; }
        [Column("CallUserFId")]
        public int? CallUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string UniqueString { get; set; }
        public Currency Currency { get; set; }
        public TransferType TransferType { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public Branch FromBranch { get; set; }
        public Branch ToBranch { get; set; }
        public User CompleteUser { get; set; }
        public User CallUser { get; set; }
        public Card Card { get; set; }
        [Column("BranchFId")]
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}

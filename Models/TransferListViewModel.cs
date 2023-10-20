using SCustomers.Dtos;
using System.Collections.Generic;

namespace SCustomers.Models
{
    public class TransferListViewModel
    {
        public List<TransferDto> Transfers { get; set; } = new List<TransferDto>();
    }
}

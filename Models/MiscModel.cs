using SCustomers.Entities;
using System.Collections.Generic;

namespace SCustomers.Models
{
    public class MiscModel
    {
        public List<Currency> Currencies { get; set; }
        public List<TransferStatus> TransferStatuses { get; set; }
        public List<TransferType> TransferTypes { get; set; }
        public List<Interval> Intervals { get; set; }
        public List<Counter> Counters { get; set; }
        public List<Branch> Branches { get; set; }
        public List<UserLog> UserLogs { get; set; }
        public List<Card> Cards { get; set; }
        public List<Title> Titles { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TCards")]
    public class Card
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
    }
}

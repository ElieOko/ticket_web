using System.ComponentModel.DataAnnotations.Schema;

namespace SCustomers.Entities
{
    [Table("TTitles")]
    public class Title
    {
        public int TitleId { get; set; }
        public string TitleName { get; set; }
        public string DisplayName { get; set; }
    }
}

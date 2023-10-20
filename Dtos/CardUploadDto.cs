using Microsoft.AspNetCore.Http;

namespace SCustomers.Dtos
{
    public class CardUploadDto
    {
        public int? TransferId { get; set; }

        public IFormFile File { get; set; }
    }
}

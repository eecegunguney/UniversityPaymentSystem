using Microsoft.AspNetCore.Http;
namespace UniversityPaymentSystem.Domain.DTOs
{
    public class AddTuitionBatchRequest
    {

        public IFormFile File { get; set; }
        public string Term { get; set; }

    }
}
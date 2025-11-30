using System.ComponentModel.DataAnnotations;

namespace UniversityPaymentSystem.Domain.DTOs
{
    public class PayTuitionRequestDto
    {
        [Required]
        public string StudentNo { get; set; }
        [Required]
        public string Term { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
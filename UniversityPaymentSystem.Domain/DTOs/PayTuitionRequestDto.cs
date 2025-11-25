namespace UniversityPaymentSystem.Domain.DTOs
{
    public class PayTuitionRequestDto
    {
        public string StudentNo { get; set; }
        public string Term { get; set; }
        public decimal Amount { get; set; }
    }
}
namespace UniversityPaymentSystem.Domain.DTOs
{
    public class AddTuitionRequestDto
    {
        public string StudentNo { get; set; }
        public string Term { get; set; }
        public decimal TuitionAmount { get; set; }
    }
}
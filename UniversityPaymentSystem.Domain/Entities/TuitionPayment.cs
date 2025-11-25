namespace UniversityPaymentSystem.Domain.Entities
{
    public class TuitionPayment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        
        public string Term { get; set; }
        public decimal TuitionTotal { get; set; } 
        public decimal PaidAmount { get; set; } 
        public string TransactionStatus { get; set; } 


        public DateTime PaymentDate { get; set; }
    }
}
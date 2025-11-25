namespace UniversityPaymentSystem.Domain.Entities
{
    public class Student
    {
    
        public int Id { get; set; }

        
        public string StudentNo { get; set; } 

        public string FullName { get; set; }

        
        public string TCKimlik { get; set; }
        public bool Status { get; set; } 


        public ICollection<TuitionPayment> Payments { get; set; }
    }
}
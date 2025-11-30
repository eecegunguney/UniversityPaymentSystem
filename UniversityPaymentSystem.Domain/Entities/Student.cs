

namespace UniversityPaymentSystem.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Student
    {
       
        public int Id { get; set; }

        [Required]
        public string StudentNo { get; set; }
        [Required]
        public string FullName { get; set; }



        [Required]
        public string TCKimlik { get; set; }
        
        public bool Status { get; set; } 


        public ICollection<TuitionPayment> Payments { get; set; }
    }
}
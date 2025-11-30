using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UniversityPaymentSystem.Domain.DTOs
{
    public class AddStudentRequestDto
    {
        [Required]
        public string StudentNo { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string TCKimlik { get; set; }
    }
}

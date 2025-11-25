using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;

namespace UniversityPaymentSystem.Application.Interfaces
{

    using Microsoft.AspNetCore.Http;

    public interface ITuitionService
    {
       
        Task<TuitionQueryResponseDto> QueryTuitionStatusAsync(string studentNo);

       
        Task<string> PayTuitionAsync(PayTuitionRequestDto request);

        
        Task<string> AddTuitionAsync(string studentNo, string term, decimal tuitionAmount);

        
        Task<string> AddTuitionBatchAsync(IFormFile csvFile, string term);

        
        Task<List<Student>> GetUnpaidTuitionStudentsAsync(string term);

        
        Task<decimal> RetrieveStudentGPAStatusAsync(string tcKimlik);
    }
}
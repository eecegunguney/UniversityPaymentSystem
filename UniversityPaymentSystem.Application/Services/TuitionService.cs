using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;
using Microsoft.AspNetCore.Http; 

namespace UniversityPaymentSystem.Application.Services
{
    public class TuitionService : ITuitionService
    {
        

        public Task<TuitionQueryResponseDto> QueryTuitionStatusAsync(string studentNo)
        {
            
            return Task.FromResult(new TuitionQueryResponseDto { TuitionTotal = 0, Balance = 0 });
        }

        public Task<string> PayTuitionAsync(PayTuitionRequestDto request)
        {
            
            return Task.FromResult("SUCCESS");
        }

        public Task<string> AddTuitionAsync(string studentNo, string term, decimal tuitionAmount)
        {
            
            return Task.FromResult("Tuition Added");
        }

        public Task<string> AddTuitionBatchAsync(IFormFile csvFile, string term)
        {
            
            return Task.FromResult("Batch Processing Started");
        }

        public Task<List<Student>> GetUnpaidTuitionStudentsAsync(string term)
        {
           
            return Task.FromResult(new List<Student>());
        }

        public Task<decimal> RetrieveStudentGPAStatusAsync(string tcKimlik)
        {
            
            return Task.FromResult(3.5m);
        }
    }
}
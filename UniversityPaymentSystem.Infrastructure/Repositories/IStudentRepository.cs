using UniversityPaymentSystem.Domain.Entities;

namespace UniversityPaymentSystem.Infrastructure.Repositories
{
    public interface IStudentRepository
    {
        
        Task<Student> GetStudentByNoAsync(string studentNo);


        Task<TuitionPayment> AddPaymentAsync(TuitionPayment payment);

        
        Task<List<Student>> GetStudentsWithUnpaidTuitionAsync(string term);

        
        Task AddOrUpdateTuitionBatchAsync(IEnumerable<TuitionPayment> payments);
    }
}
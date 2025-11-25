using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityPaymentSystem.Domain.Entities;
using UniversityPaymentSystem.Infrastructure.Data;

namespace UniversityPaymentSystem.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentByNoAsync(string studentNo)
        {
            return await _context.Students
                                 .FirstOrDefaultAsync(s => s.StudentNo == studentNo);
        }

        public async Task<List<Student>> GetStudentsWithUnpaidTuitionAsync(string term)
        {
            
            return await _context.TuitionPayments
                                 .Where(tp => tp.Term == term && tp.TuitionTotal > tp.PaidAmount)
                                 .Select(tp => tp.Student) 
                                 .Distinct()
                                 .ToListAsync();
        }

      
        public async Task<TuitionPayment> AddPaymentAsync(TuitionPayment payment)
        {
            await _context.TuitionPayments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        
        public async Task AddOrUpdateTuitionBatchAsync(IEnumerable<TuitionPayment> payments)
        {
            foreach (var payment in payments)
            {
               
                var existingPayment = await _context.TuitionPayments
                    .FirstOrDefaultAsync(tp => tp.StudentId == payment.StudentId && tp.Term == payment.Term);

                if (existingPayment != null)
                {
                    
                    existingPayment.TuitionTotal = payment.TuitionTotal;
                    _context.TuitionPayments.Update(existingPayment);
                }
                else
                {
                    
                    await _context.TuitionPayments.AddAsync(payment);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
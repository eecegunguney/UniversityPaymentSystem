using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;
using UniversityPaymentSystem.Infrastructure.Data;

namespace UniversityPaymentSystem.Application.Services
{
    public class TuitionService : ITuitionService
    {
        private readonly ApplicationDbContext _context;

        public TuitionService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<string> AddStudentAsync(AddStudentRequestDto request)
        {
           
            var existingStudent = await _context.Students
                                                .FirstOrDefaultAsync(s => s.StudentNo == request.StudentNo);

            if (existingStudent != null)
            {
                return $"ERROR: Student with No {request.StudentNo} already exists.";
            }

            
            var newStudent = new Student
            {
                StudentNo = request.StudentNo,
                FullName = request.FullName,
                TCKimlik= request.TCKimlik 
            };

            _context.Students.Add(newStudent);

            try
            {
                
                await _context.SaveChangesAsync();
                return "Student Added Successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Database write failed. {ex.Message}";
            }
        }


        public async Task<TuitionQueryResponseDto> QueryTuitionStatusAsync(string studentNo)
        {
            var student = await _context.Students
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(s => s.StudentNo == studentNo);

            if (student == null)
            {
                return new TuitionQueryResponseDto { TuitionTotal = 0, Balance = 0 };
            }

            var tuitionRecords = await _context.TuitionPayments
                                             .AsNoTracking()
                                             .Where(tp => tp.StudentId == student.Id)
                                             .ToListAsync();

            var totalTuition = tuitionRecords.Sum(tp => tp.TuitionTotal);
            var totalPaid = tuitionRecords.Sum(tp => tp.PaidAmount);

            return new TuitionQueryResponseDto
            {
                TuitionTotal = totalTuition,
                Balance = totalTuition - totalPaid
            };
        }

   
        public async Task<string> AddTuitionAsync(string studentNo, string term, decimal tuitionAmount)
        {
            var student = await _context.Students
                                       .FirstOrDefaultAsync(s => s.StudentNo == studentNo);

            if (student == null)
            {
                return "ERROR: Student not found.";
            }

            var newPaymentRecord = new TuitionPayment
            {
                StudentId = student.Id,
                Term = term,
                TuitionTotal = tuitionAmount,
                PaidAmount = 0.00m,
                TransactionStatus = "PENDING",
                PaymentDate = DateTime.UtcNow
            };

            _context.TuitionPayments.Add(newPaymentRecord);

            try
            {
                await _context.SaveChangesAsync();

                return "Tuition Added Successfully";
            }
            catch (Exception ex)
            {
                return $"ERROR: Database write failed. {ex.Message}";
            }
        }


        public async Task<string> PayTuitionAsync(PayTuitionRequestDto request)
        {
            var student = await _context.Students
                                        .FirstOrDefaultAsync(s => s.StudentNo == request.StudentNo);

            if (student == null)
            {
                return "ERROR: Student not found for payment.";
            }

            var tuitionRecord = await _context.TuitionPayments
                                              .Where(tp => tp.StudentId == student.Id && tp.Term == request.Term)
                                              .FirstOrDefaultAsync();

            if (tuitionRecord == null)
            {
                return "ERROR: Tuition record for the specified term was not found.";
            }

            if (request.Amount <= 0)
            {
                return "ERROR: Payment amount must be positive.";
            }

            tuitionRecord.PaidAmount += request.Amount;

            if (tuitionRecord.PaidAmount >= tuitionRecord.TuitionTotal)
            {
                tuitionRecord.TransactionStatus = "COMPLETED";
            }
            else
            {
                tuitionRecord.TransactionStatus = "PARTIALLY_PAID";
            }

            try
            {
                await _context.SaveChangesAsync();

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return $"ERROR: Payment failed during database update. {ex.Message}";
            }
        }


        public async Task<string> AddTuitionBatchAsync(IFormFile csvFile, string term)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return "ERROR: CSV file is empty or null.";
            }

            var temporaryRecords = new List<(string StudentNo, decimal TuitionAmount)>();
            var studentNumbers = new List<string>();

            try
            {
                using (var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
                {
                    await reader.ReadLineAsync();
                    string line;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var values = line.Split(',');

                        if (values.Length < 2) continue;

                        string studentNo = values[0].Trim();
                        if (decimal.TryParse(values[1].Trim(), out decimal tuitionAmount) && tuitionAmount > 0)
                        {
                            studentNumbers.Add(studentNo);
                            temporaryRecords.Add((studentNo, tuitionAmount)); 
                        }
                    }
                }

                
                var existingStudents = await _context.Students
                                                    .Where(s => studentNumbers.Contains(s.StudentNo))
                                                    .ToDictionaryAsync(s => s.StudentNo, s => s.Id);

                var finalRecordsToSave = new List<TuitionPayment>();

               
                foreach (var tempRecord in temporaryRecords)
                {
                    
                    if (existingStudents.TryGetValue(tempRecord.StudentNo, out int studentId))
                    {
                        finalRecordsToSave.Add(new TuitionPayment
                        {
                            StudentId = studentId,
                            Term = term,
                            TuitionTotal = tempRecord.TuitionAmount, 
                            PaidAmount = 0.00m,
                            TransactionStatus = "PENDING",
                            PaymentDate = DateTime.UtcNow
                        });
                    }
                }

                
                _context.TuitionPayments.AddRange(finalRecordsToSave);
                await _context.SaveChangesAsync();

                return $"Batch Processing Completed. {finalRecordsToSave.Count} records successfully added.";
            }
            catch (Exception ex)
            {
                return $"ERROR during batch processing: {ex.Message}";
            }
        }


        public async Task<List<Student>> GetUnpaidTuitionStudentsAsync(string term)
        {
            try
            {
                
                var unpaidStudentIds = await _context.TuitionPayments
                    .Where(tp => tp.Term == term && tp.TuitionTotal > tp.PaidAmount)
                    .Select(tp => tp.StudentId)
                    .Distinct() 
                    .ToListAsync();

                
                var unpaidStudents = await _context.Students
                    .Where(s => unpaidStudentIds.Contains(s.Id))
                    .AsNoTracking()
                    .ToListAsync();

                return unpaidStudents;
            }
            catch (Exception ex)
            {

                return new List<Student>();
            }
        }

    
    }
}
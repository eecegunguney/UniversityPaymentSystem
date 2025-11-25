using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;
using Microsoft.AspNetCore.Http; 


[ApiController]
[Route("api/v1/[controller]")] 
public class TuitionController : ControllerBase
{
    private readonly ITuitionService _tuitionService;
    private readonly ILogger<TuitionController> _logger; 

    public TuitionController(ITuitionService tuitionService, ILogger<TuitionController> logger)
    {
        _tuitionService = tuitionService;
        _logger = logger;
    }

    
    [HttpGet("query")]
    [ProducesResponseType(typeof(TuitionQueryResponseDto), 200)]
    [ProducesResponseType(429)]
    [AllowAnonymous]
    public async Task<IActionResult> QueryTuition([FromQuery] string studentNo)
    {

        _logger.LogInformation("QueryTuition - Request: {StudentNo}, SourceIp: {IP}", studentNo, HttpContext.Connection.RemoteIpAddress);

        if (string.IsNullOrEmpty(studentNo))
        {
            return BadRequest("Student Number is required.");
        }

        

        var result = await _tuitionService.QueryTuitionStatusAsync(studentNo);


        _logger.LogInformation("QueryTuition - Success: {StudentNo}, Latency: {Latency}ms", studentNo, 100);

        return Ok(result);
    }


    [HttpPost("pay")]
    [ProducesResponseType(typeof(string), 200)]
    [AllowAnonymous]
    public async Task<IActionResult> PayTuition([FromBody] PayTuitionRequestDto request)
    {
        
        _logger.LogInformation("PayTuition - Request: {StudentNo}", request.StudentNo);

   
        var status = await _tuitionService.PayTuitionAsync(request);

       
        return Ok(new { TransactionStatus = status });
    }

  
    [HttpPost("admin/add")]
    [Authorize(Roles = "Admin")] 
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(401)] 
    public async Task<IActionResult> AddTuition([FromBody] AddTuitionRequestDto request)
    {
        

        var status = await _tuitionService.AddTuitionAsync(
            request.StudentNo,
            request.Term,
            request.TuitionAmount
        );

        return Ok(new { TransactionStatus = status });
    }


    [HttpPost("admin/add/batch")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> AddTuitionBatch([FromForm] AddTuitionBatchRequest request)
    {
        
        var file = request.File;
        var term = request.Term;

        if (file == null || file.Length == 0)
        {
            return BadRequest("CSV file is required.");
        }

        var status = await _tuitionService.AddTuitionBatchAsync(file, term);

        return Ok(new { TransactionStatus = status });
    }

    [HttpGet("admin/unpaid")]
    [Authorize(Roles = "Admin")] 
    [ProducesResponseType(typeof(List<Student>), 200)]
    public async Task<IActionResult> GetUnpaidStudents([FromQuery] string term, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {


        var students = await _tuitionService.GetUnpaidTuitionStudentsAsync(term);

        
        var pagedStudents = students.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


        _logger.LogInformation("UnpaidStudents - Count: {Count}", pagedStudents.Count);

        return Ok(pagedStudents);
    }

   
    [HttpGet("gpa")]
    [Authorize(Roles ="Admin")] 
    public async Task<IActionResult> RetrieveGPA([FromQuery] string tcKimlik)
    {
        if (string.IsNullOrEmpty(tcKimlik))
        {
            return BadRequest("TC Kimlik No is required.");
        }
        var gpa = await _tuitionService.RetrieveStudentGPAStatusAsync(tcKimlik);
        return Ok(new { GPA = gpa });
    }
}
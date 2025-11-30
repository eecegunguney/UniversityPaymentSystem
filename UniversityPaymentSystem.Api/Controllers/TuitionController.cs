using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;


[ApiController]
[Route("api/v1/[controller]")] 
public class TuitionController : ControllerBase
{
    private readonly ITuitionService _tuitionService;
    private readonly ILogger<TuitionController> _logger;
    private readonly IMemoryCache _cache;

    public TuitionController(ITuitionService tuitionService, ILogger<TuitionController> logger, IMemoryCache memoryCache)
    {
        _tuitionService = tuitionService;
        _logger = logger;
        _cache = memoryCache;
    }

    [HttpPost("admin/student/add")]
    [Authorize(Roles = "Admin")] 
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddStudent([FromBody] AddStudentRequestDto request)
    {
        var status = await _tuitionService.AddStudentAsync(request);

        if (status.StartsWith("ERROR"))
        {
            return BadRequest(status);
        }
        return Ok(new { TransactionStatus = status });
    }


    [HttpGet("mobileApp/query")]
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

    [HttpGet("bankingApp/query")]
    [Authorize(Roles = "Banking")]
    [ProducesResponseType(typeof(TuitionQueryResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> QueryTuitionForBanking([FromQuery] string studentNo)
    {
        _logger.LogInformation("QueryTuitionForBanking - Request: {StudentNo}", studentNo);

        if (string.IsNullOrEmpty(studentNo))
        {
            return BadRequest("Student Number is required.");
        }


        var result = await _tuitionService.QueryTuitionStatusAsync(studentNo);

        return Ok(result);
    }

    [HttpPost("bankingApp/pay")]
    [ProducesResponseType(typeof(string), 200)]
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
    [ProducesResponseType(401)]
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
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUnpaidStudents([FromQuery] string term, [FromQuery] int pageNumber , [FromQuery] int pageSize)
    {


        var students = await _tuitionService.GetUnpaidTuitionStudentsAsync(term);

        
       


        _logger.LogInformation("UnpaidStudents - Count: {Count}", students.Count);

        return Ok(students);
    }

   

}
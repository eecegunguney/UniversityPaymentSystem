using Microsoft.AspNetCore.Mvc;
using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;

namespace UniversityPaymentSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

       
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Kullanıcı bilgileri eksik.");
            }

            var token = await _authService.GenerateToken(loginDto);

            if (string.IsNullOrEmpty(token))
            {
               
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

 
            return Ok(new { Token = token });
        }
    }
}
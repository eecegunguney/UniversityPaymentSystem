using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Domain.DTOs;


namespace UniversityPaymentSystem.Application.Services
{
   
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        
        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public Task<string> GenerateToken(UserLoginDto loginDto)
        {
            string role = string.Empty;

           
            if (loginDto.Username.ToLower() == "admin" && loginDto.Password == "password")
            {
                role = "Admin";
            }
            else if (loginDto.Username.ToLower() == "banking" && loginDto.Password == "password")
            {
                role = "Banking";
            }
            else
            {
               
                return Task.FromResult(string.Empty);
            }

    
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginDto.Username),
                new Claim(ClaimTypes.Name, loginDto.Username),
                new Claim(ClaimTypes.Role, role), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
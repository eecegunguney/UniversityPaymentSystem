using System;
using System.Collections.Generic;
using System.Text;
using UniversityPaymentSystem.Domain.DTOs;
using UniversityPaymentSystem.Domain.Entities;


namespace UniversityPaymentSystem.Application.Interfaces
{
    using Microsoft.AspNetCore.Http;
    public interface IAuthService
    {

        Task<string> GenerateToken(UserLoginDto loginDto);
    }
}

using UniversityPaymentSystem.Application.Interfaces;
using UniversityPaymentSystem.Application.Services;
using UniversityPaymentSystem.Infrastructure.Data;
using UniversityPaymentSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();


builder.Services.AddScoped<IStudentRepository, StudentRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>

    options.UseNpgsql(connectionString));



builder.Services.AddScoped<ITuitionService, TuitionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "University Tuition API", Version = "v1" });
    options.DocumentFilter<GatewayUrlDocumentFilter>();

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

var app = builder.Build();




app.UseSwagger();
app.UseSwaggerUI(c =>
{
    
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UTPS API v1");

    
    c.RoutePrefix = string.Empty;

    
    c.DocumentTitle = "UTPS API - Gateway";
});


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
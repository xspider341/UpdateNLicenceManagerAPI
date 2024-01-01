using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UpdateNLicenceManagerAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Text;
using UpdateNLicenceManagerAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// JWT Kimlik Doğrulaması Yapılandırması
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey")) //JWT için buraya key üretip girecek.
        };
    });

// MongoDB Yapılandırması
builder.Services.AddSingleton<IMongoClient, MongoClient>(
    serviceProvider => new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));


var filePath = builder.Configuration.GetValue<string>("FileTransferPath");

// Yapılandırmayı servis konteynerine ekle
#pragma warning disable CS8601 // Possible null reference assignment.
builder.Services.AddSingleton(new FileTransferOptions { Path = filePath });
#pragma warning restore CS8601 // Possible null reference assignment.

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();

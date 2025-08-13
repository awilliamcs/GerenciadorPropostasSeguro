using AutoMapper;
using GPS.ContratacaoService.Application.Interfaces;
using GPS.ContratacaoService.Application.Mapping;
using GPS.ContratacaoService.Application.Services;
using GPS.ContratacaoService.Domain.Interfaces;
using GPS.ContratacaoService.Infrastructure;
using GPS.ContratacaoService.Infrastructure.Clients;
using GPS.ContratacaoService.Infrastructure.Repositories;
using GPS.CrossCutting.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContratacaoDb"))
);

builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

// Configuração JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IPropostaClient, PropostaClient>();

builder.Services.AddScoped<IContratacaoService, ContratacaoService>();
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();

var mapperConfigExpression = new MapperConfigurationExpression();
mapperConfigExpression.AddProfile<ContratacaoProfile>();

var mapperConfig = new MapperConfiguration(mapperConfigExpression);
var mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();


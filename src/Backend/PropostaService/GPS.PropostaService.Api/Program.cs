using GPS.CrossCutting.Messaging;
using GPS.PropostaService.Application.Interfaces;
using GPS.PropostaService.Application.Mapping;
using GPS.PropostaService.Application.Services;
using GPS.PropostaService.Application.Consumers;
using GPS.PropostaService.Domain.Interfaces;
using GPS.PropostaService.Infrastructure;
using GPS.PropostaService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PropostaDb"))
);

builder.Services.AddMassTransitWithRabbitMqFromAssemblies(builder.Configuration, typeof(SalvarPropostaConsumer).Assembly);


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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPropostaService, PropostaService>();
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddAutoMapper(typeof(PropostaProfile));

var app = builder.Build();

// Executar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();


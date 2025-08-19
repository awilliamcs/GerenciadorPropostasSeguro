using GPS.AutenticacaoService.Application.Interfaces;
using GPS.AutenticacaoService.Application.Services;
using GPS.AutenticacaoService.Application.Consumers;
using GPS.AutenticacaoService.Domain.Entidades;
using GPS.AutenticacaoService.Domain.Interfaces;
using GPS.AutenticacaoService.Infrastructure;
using GPS.AutenticacaoService.Infrastructure.Repositories;
using GPS.CrossCutting.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AutenticacaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AutenticacaoDb"))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AutenticacaoDbContext>()
.AddDefaultTokenProviders();

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

builder.Services.AddMassTransitWithRabbitMqFromAssemblies(builder.Configuration, typeof(PessoaCriadaEventConsumer).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
builder.Services.AddScoped<IAutenticacaoRepository, AutenticacaoRepository>();
builder.Services.AddScoped<IRegistroUsuarioSagaOrchestrator, RegistroUsuarioSagaOrchestrator>();

var app = builder.Build();

// Executar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AutenticacaoDbContext>();
    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
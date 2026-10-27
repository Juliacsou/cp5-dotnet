using Oracle.EntityFrameworkCore.Infrastructure;
using System.Text;
using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using CP5.Infrastructure.Data;
using CP5.Infrastructure.Data.Repositories;
using CP5.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
        Description = "Informe o JWT obtido em POST /api/auth"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type=ReferenceType.SecurityScheme, Id="Bearer" } }, Array.Empty<string>() }
    });
});
var connection=builder.Configuration.GetConnectionString("Oracle")??throw new InvalidOperationException("Configure ConnectionStrings:Oracle no appsettings.Development.json.");
// O Oracle da FIAP pode não aceitar literais booleanos TRUE/FALSE gerados pelo provider.
// Utilizamos a compatibilidade com Oracle 19c do próprio Oracle.EntityFrameworkCore.
builder.Services.AddDbContext<ApplicationContext>(o =>
    o.UseOracle(connection, oracle =>
        oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));
builder.Services.AddScoped<ILivroRepository,LivroRepository>();
builder.Services.AddScoped<IEmprestimoRepository,EmprestimoRepository>();
builder.Services.AddScoped<IRepository<LivroEntity>>(sp=>sp.GetRequiredService<ILivroRepository>());
builder.Services.AddScoped<IRepository<EmprestimoEntity>>(sp=>sp.GetRequiredService<IEmprestimoRepository>());
builder.Services.AddScoped<CadastroUseCase<LivroEntity>>();
builder.Services.AddScoped<CadastroUseCase<EmprestimoEntity>>();
builder.Services.AddScoped<IEnderecoService,EnderecoService>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<UserUseCase>();
var secret=builder.Configuration["Secretkey"]??throw new InvalidOperationException("Configure Secretkey no appsettings.Development.json (mínimo 32 caracteres).");
if(Encoding.UTF8.GetByteCount(secret)<32)throw new InvalidOperationException("Secretkey deve conter pelo menos 32 bytes.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>options.TokenValidationParameters=new TokenValidationParameters { ValidateIssuer=false,ValidateAudience=false,ValidateIssuerSigningKey=true,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),ValidateLifetime=true,ClockSkew=TimeSpan.Zero });
builder.Services.AddAuthorization();
var app=builder.Build();

app.UseSwagger();app.UseSwaggerUI();
app.UseAuthentication();app.UseAuthorization();
app.MapControllers();app.Run();
public partial class Program {}

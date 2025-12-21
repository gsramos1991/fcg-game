using FCG.Game.Application.Services;
﻿using FCG.Game.Application.Services.Interfaces;
﻿using FCG.Game.Application.Repositories;
﻿using FCG.Game.Infrastructure.Repositories;
﻿using FCG.Game.Infrastructure.Data;
﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
﻿using Microsoft.IdentityModel.Tokens;
﻿using System.Text;
﻿using Microsoft.EntityFrameworkCore;
﻿using FCG.Game.Application.Clients;
﻿using FCG.Game.Infrastructure.Clients;
﻿
﻿var builder = WebApplication.CreateBuilder(args);
﻿
﻿// ============================================
﻿// CONFIGURAÇÃO DE SERVIÇOS
﻿// ============================================
﻿
﻿// Controllers
﻿builder.Services.AddControllers();
﻿builder.Services.AddEndpointsApiExplorer();
﻿
﻿// Swagger
﻿builder.Services.AddSwaggerGen(c =>
﻿{
﻿    c.SwaggerDoc("v1", new()
﻿    {
﻿        Title = "FCG Games Microservice",
﻿        Version = "v1",
﻿        Description = "API de Jogos com SQL Server" // Updated description
﻿    });
﻿
﻿    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
﻿    {
﻿        Name = "Authorization",
﻿        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
﻿        Scheme = "bearer",
﻿        BearerFormat = "JWT",
﻿        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
﻿        Description = "Insira o token JWT aqui."
﻿    });
﻿
﻿    c.AddSecurityRequirement(new()
﻿    {
﻿        {
﻿            new()
﻿            {
﻿                Reference = new()
﻿                {
﻿                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
﻿                    Id = "Bearer"
﻿                }
﻿            },
﻿            Array.Empty<string>()
﻿        }
﻿    });
﻿});
﻿
﻿// ============================================
﻿// ENTITY FRAMEWORK CORE
﻿// ============================================
﻿var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
﻿builder.Services.AddDbContext<GameDbContext>(options =>
﻿    options.UseSqlServer(connectionString));
﻿
﻿// ============================================
﻿// REPOSITORIES
﻿// ============================================
﻿builder.Services.AddScoped<IGameRepository, GameRepository>();
﻿builder.Services.AddScoped<IOrderRepository, OrderRepository>();
﻿
﻿// ============================================
﻿// APPLICATION SERVICES
﻿// ============================================
﻿builder.Services.AddScoped<IGameService, GameService>();
﻿builder.Services.AddScoped<IOrderService, OrderService>();
﻿builder.Services.AddScoped<MetricsService>();
﻿
﻿// ============================================
﻿// HTTP CLIENTS
﻿// ============================================
﻿builder.Services.AddHttpClient<IOrderApiClient, OrderApiClient>();
﻿
﻿// ============================================
﻿// BACKGROUND SERVICES
﻿// ============================================
﻿// Background Service desabilitado temporariamente (API do EventStore mudou)
﻿// Para habilitar no futuro, descomente a linha abaixo:
﻿// builder.Services.AddHostedService<EventStoreSubscriptionService>();
﻿
﻿// ============================================
﻿// JWT AUTHENTICATION
﻿// ============================================
﻿var jwtKey = builder.Configuration["Jwt:Key"]
﻿    ?? throw new InvalidOperationException("JWT Key não configurada");
﻿
﻿builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
﻿    .AddJwtBearer(options =>
﻿    {
﻿        options.TokenValidationParameters = new TokenValidationParameters
﻿        {
﻿            ValidateIssuer = true,
﻿            ValidateAudience = true,
﻿            ValidateLifetime = true,
﻿            ValidateIssuerSigningKey = true,
﻿            ValidIssuer = builder.Configuration["Jwt:Issuer"],
﻿            ValidAudience = builder.Configuration["Jwt:Audience"],
﻿            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
﻿        };
﻿    });
﻿
﻿// ============================================
﻿// CORS
﻿// ============================================
﻿builder.Services.AddCors(options =>
﻿{
﻿    options.AddPolicy("AllowAll", policy =>
﻿    {
﻿        policy.AllowAnyOrigin()
﻿              .AllowAnyMethod()
﻿              .AllowAnyHeader();
﻿    });
﻿});
﻿
﻿// ============================================
﻿// HEALTH CHECKS
﻿// ============================================
﻿builder.Services.AddHealthChecks()
﻿    .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "db", "data" });
﻿
﻿// ============================================
﻿// BUILD DA APLICAÇÃO
﻿// ============================================
﻿var app = builder.Build();
﻿
﻿// ============================================
﻿// INICIALIZAÇÃO DOS SERVIÇOS
﻿// ============================================
﻿
﻿app.Logger.LogInformation("🚀 Iniciando FCG Games Microservice...");
﻿
﻿// ============================================
﻿// MIDDLEWARE PIPELINE
﻿// ============================================
﻿
﻿app.UseSwagger();
﻿app.UseSwaggerUI(c =>
﻿{
﻿    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG Games API v1");
﻿    c.RoutePrefix = "swagger";
﻿});
﻿
﻿app.Logger.LogInformation("📚 Swagger disponível em: http://localhost:5002/swagger");
﻿
﻿app.UseHttpsRedirection();
﻿app.UseCors("AllowAll");
﻿app.UseAuthentication();
﻿app.UseAuthorization();
﻿
﻿app.MapControllers();
﻿app.MapHealthChecks("/health");
﻿
﻿// ============================================
﻿// LOG DE INICIALIZAÇÃO
﻿// ============================================
﻿app.Logger.LogInformation("================================================");
﻿app.Logger.LogInformation("✨ FCG Games Microservice PRONTO! ✨");
﻿app.Logger.LogInformation("================================================");
﻿app.Logger.LogInformation("🌐 API: http://localhost:5002");
﻿app.Logger.LogInformation("📚 Swagger: http://localhost:5002/swagger");
﻿app.Logger.LogInformation("💚 Health: http://localhost:5002/health");
﻿app.Logger.LogInformation("================================================");
﻿
﻿app.Run();
﻿
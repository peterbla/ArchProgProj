using System.Security.Cryptography.X509Certificates;

using Backend.DatabaseModels;
using Backend.Services;
using Backend;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

using Models;

using Supabase;
using Newtonsoft.Json;

const string supabaseUrl = "https://jjctsjkcpqocbinkucvv.supabase.co";
const string supabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImpqY3RzamtjcHFvY2Jpbmt1Y3Z2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzYwOTM5MDAsImV4cCI6MjA1MTY2OTkwMH0.HyYXyn1a-SePbP_4U3gds4gWf42-IBOe3K8N56cHrps";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<WeightHistoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
          new string[] { }
        }
    });
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(TokenService.GenerateSecretByte()),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("user", policy => policy.RequireRole("user"));
});

SupabaseOptions supabaseOptions = new()
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true,
};
builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseApiKey, supabaseOptions));


// Dodanie kontrolerów
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

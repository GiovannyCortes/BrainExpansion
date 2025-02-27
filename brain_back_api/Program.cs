using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using brain_back_application;
using brain_back_infrastructure;
using brain_back_application.Helpers;
using brain_back_domain.Enumerations;

var builder = WebApplication.CreateBuilder(args);

// Security
builder.Services.AddSingleton<HelperOAuthToken>();

HelperOAuthToken helperOAuth = new HelperOAuthToken(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = helperOAuth.Issuer,
            ValidAudience = helperOAuth.Audience,
            IssuerSigningKey = helperOAuth.GetKeyToken()
        };
    });

// Add services to the container.
Dictionary<EDBSelected, string> cnStrings = new Dictionary<EDBSelected, string> {
    { EDBSelected.SqlServer, builder.Configuration.GetConnectionString("SqlServer")! },
    { EDBSelected.PostGress, builder.Configuration.GetConnectionString("PostGress")! }
};

// Configurar servicios de infraestructura (repositorios y DbContext)
builder.Services.AddInfrastructureServices(cnStrings, EDBSelected.PostGress);

// Configurar servicios de la capa aplicaci�n (servicios de negocio)
builder.Services.AddApplicationServices();

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowOrigin", builder => {
        builder.WithOrigins("http://localhost:49953").AllowAnyHeader().AllowAnyMethod();
    });
});

//builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Brain Generator API"
        , Version = "v1"
        , Description = "API de la aplicación web Brain Expansion"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API OAuth Brain-G");
        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowOrigin");

app.MapControllers();

app.Run();
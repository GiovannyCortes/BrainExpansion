using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using brain_back_application.Helpers;
using brain_back_domain.Enumerations;
using brain_back_application.Interfaces;
using brain_back_application.Services;

using brain_back_infrastructure.Data;
using brain_back_infrastructure.Interfaces;

try
{
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

    EDBSelected dbSelected = EDBSelected.PostGress;

    Dictionary<EDBSelected, string> cnStrings = new Dictionary<EDBSelected, string> {
        { EDBSelected.SqlServer, builder.Configuration.GetConnectionString("SqlServer")! },
        { EDBSelected.PostGress, builder.Configuration.GetConnectionString("PostGress")! }
    };

    switch (dbSelected)
    {
        case EDBSelected.SqlServer:
            builder.Services.AddDbContext<BrainContext>(options => options.UseSqlServer(cnStrings[EDBSelected.SqlServer])); // SQL Server

            // Registrar repositorios
            builder.Services.AddTransient<IUserRepository, brain_back_infrastructure.Repositories_SqlServer.UserRepository>();
            builder.Services.AddTransient<IQuestionRepository, brain_back_infrastructure.Repositories_SqlServer.QuestionRepository>();
            break;
        case EDBSelected.PostGress:
            builder.Services.AddDbContext<BrainContext>(options => options.UseNpgsql(cnStrings[EDBSelected.PostGress])); // PostgreSQL

            // Registrar repositorios
            builder.Services.AddTransient<IUserRepository, brain_back_infrastructure.Repositories_PostGress.UserRepository>();
            builder.Services.AddTransient<IQuestionRepository, brain_back_infrastructure.Repositories_PostGress.QuestionRepository>();
            break;
    }

    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IQuestionService, QuestionService>();


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
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
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
}
catch (Exception ex)
{
    Console.WriteLine($"Error crítico: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
}
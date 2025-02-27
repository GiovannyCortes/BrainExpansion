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

int Line = 0; // Move the declaration of Line outside the try block

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Security
    Line = 1;
    builder.Services.AddSingleton<HelperOAuthToken>();

    Line = 2;
    HelperOAuthToken helperOAuth = new HelperOAuthToken(builder.Configuration);

    Line = 3;
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
    Line = 4;
    EDBSelected dbSelected = EDBSelected.PostGress;

    Dictionary<EDBSelected, string> cnStrings = new Dictionary<EDBSelected, string> {
            { EDBSelected.SqlServer, builder.Configuration.GetConnectionString("SqlServer")! },
            { EDBSelected.PostGress, builder.Configuration.GetConnectionString("PostGress")! }
        };

    Line = 5;
    switch (dbSelected)
    {
        case EDBSelected.SqlServer:
            Line = 6;
            builder.Services.AddDbContext<BrainContext>(options => options.UseSqlServer(cnStrings[EDBSelected.SqlServer])); // SQL Server

            // Registrar repositorios
            builder.Services.AddTransient<IUserRepository, brain_back_infrastructure.Repositories_SqlServer.UserRepository>();
            builder.Services.AddTransient<IQuestionRepository, brain_back_infrastructure.Repositories_SqlServer.QuestionRepository>();
            break;
        case EDBSelected.PostGress:
            Line = 7;
            builder.Services.AddDbContext<BrainContext>(options => options.UseNpgsql(cnStrings[EDBSelected.PostGress])); // PostgreSQL

            // Registrar repositorios
            builder.Services.AddTransient<IUserRepository, brain_back_infrastructure.Repositories_PostGress.UserRepository>();
            builder.Services.AddTransient<IQuestionRepository, brain_back_infrastructure.Repositories_PostGress.QuestionRepository>();
            break;
    }
    Line = 8;
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IQuestionService, QuestionService>();

    Line = 9;
    builder.Services.AddControllersWithViews();

    Line = 10;
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowOrigin", builder =>
        {
            builder.WithOrigins("http://localhost:49953").AllowAnyHeader().AllowAnyMethod();
        });
    });

    //builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    Line = 11;
    builder.Services.AddEndpointsApiExplorer();

    Line = 12;
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Brain Generator API"
            ,
            Version = "v1"
            ,
            Description = "API de la aplicación web Brain Expansion"
        });
    });

    Line = 13;
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    Line = 14;
    if (app.Environment.IsDevelopment())
    {
        Line = 15;
        app.UseSwagger();

        Line = 16;
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API OAuth Brain-G");
            options.RoutePrefix = "";
        });
    }

    Line = 17;
    app.UseHttpsRedirection();

    Line = 18;
    app.UseAuthentication();

    Line = 19;
    app.UseAuthorization();

    Line = 20;
    app.UseCors("AllowOrigin");

    app.Urls.Add("http://0.0.0.0:8080");

    Line = 21;
    app.MapControllers();

    Line = 22;
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error en la línea {Line}");
    Console.WriteLine($"Error crítico: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
}
using brain_back_domain.Enumerations;
using brain_back_infrastructure.Data;
using brain_back_infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace brain_back_infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, Dictionary<EDBSelected, string> cnStrings, EDBSelected dbSelected)
        {
            // Configurar DbContext
            switch (dbSelected)
            {
                case EDBSelected.SqlServer:
                    services.AddDbContext<BrainContext>(options => options.UseSqlServer(cnStrings[EDBSelected.SqlServer])); // SQL Server
                    
                    // Registrar repositorios
                    services.AddTransient<IUserRepository, Repositories_SqlServer.UserRepository>();
                    services.AddTransient<IQuestionRepository, Repositories_SqlServer.QuestionRepository>();
                    break;
                case EDBSelected.PostGress:
                    services.AddDbContext<BrainContext>(options => options.UseNpgsql(cnStrings[EDBSelected.PostGress])); // PostgreSQL

                    // Registrar repositorios
                    services.AddTransient<IUserRepository, Repositories_PostGress.UserRepository>();
                    services.AddTransient<IQuestionRepository, Repositories_PostGress.QuestionRepository>();
                    break;
            }

        }
    }
}
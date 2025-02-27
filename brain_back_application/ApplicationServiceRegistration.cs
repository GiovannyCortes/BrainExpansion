using brain_back_application.Services;
using brain_back_application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace brain_back_application
{
    public static class ApplicationServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Registrar servicios de la capa aplicación
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IQuestionService, QuestionService>();
        }
    }
}
using Infrastructures;
using Repositories.Implements;
using Repositories.Interfaces;

namespace WebClient
{
    public static class ConfigureServices
    {
        public static IConfiguration? Configuration { get; }
        public static IServiceCollection AddWebClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ConnectionContext>(options =>
            {
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}

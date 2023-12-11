using Infrastructures;
using Repositories;

namespace WebClient
{
    public static class ConfigureServices
    {
        public static IConfiguration? Configuration { get; }

        public static IServiceCollection AddWebClientServices(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ConnectionContext>(options =>
            {
            });

            services.AddAuthentication()
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Auth/Login";
                        options.LogoutPath = "/Auth/Logout";
                    });

            //services.AddIdentityCore<User>(options =>
            //   {
            //       options.SignIn.RequireConfirmedAccount = false;
            //       options.User.RequireUniqueEmail = true;
            //       options.Password.RequireDigit = false;
            //       options.Password.RequiredLength = 6;
            //       options.Password.RequireNonAlphanumeric = false;
            //       options.Password.RequireUppercase = false;
            //       options.Password.RequireLowercase = false;
            //   })
            //   .AddEntityFrameworkStores<ConnectionContext>();

            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddRepositoryServices();

            return services;
        }

    }
}

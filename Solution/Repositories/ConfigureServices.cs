﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Implements;
using Repositories.Interfaces;

namespace Repositories
{
    public static class ConfigureServices
    {

        public static IConfiguration? Configuration { get; }
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            return services;
        }
    }
}

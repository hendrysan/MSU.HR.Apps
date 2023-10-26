using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Contexts
{
    public class ConnectionContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; }

        public ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var con = _configuration.GetSection("ConnectionString:PostgresConnection").Value;
            options.UseNpgsql(con);
        }

    }
}

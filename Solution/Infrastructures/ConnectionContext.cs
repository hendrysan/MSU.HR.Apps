using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Infrastructures
{
    public class ConnectionContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<GrantAccess> GrantAccesses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var con = _configuration.GetConnectionString("PostgreSQLConnection");
            options.UseNpgsql(con);
        }

    }
}
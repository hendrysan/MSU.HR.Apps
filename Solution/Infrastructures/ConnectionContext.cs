using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Infrastructures
{
    public class ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration) : DbContext(options)
    {
        protected readonly IConfiguration _configuration = configuration;

        public DbSet<MasterUser> MasterUsers { get; set; }
        public DbSet<GrantAccess> GrantAccesses { get; set; }
        public DbSet<MasterRole> MasterRoles { get; set; }
        public DbSet<MasterEmployee> MasterEmployees { get; set; }
        public DbSet<StagingVerify> StagingVerifies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var con = _configuration.GetConnectionString("PostgreSQLConnection");
            options.UseNpgsql(con);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        
        }

    }
}
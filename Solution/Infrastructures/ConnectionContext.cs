using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models.Entities;

namespace Infrastructures
{
    public class ConnectionContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            this._configuration = configuration;
        }

        
        public DbSet<MasterUser> MasterUsers { get; set; }
        public DbSet<GrantAccess> GrantAccesses { get; set; }
        public DbSet<MasterRole> MasterRoles { get; set; }
        public DbSet<MasterEmployee> MasterEmployees { get; set; }
        public DbSet<StagingVerify> StagingVerifies { get; set; }
        public DbSet<StagingDocumentAttendance> StagingDocumentAttendances { get; set; }
        public DbSet<StagingDocumentAttendanceDetail> StagingDocumentAttendanceDetails { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            
            var con = _configuration.GetConnectionString("PostgreSQLConnection");
            options.UseNpgsql(con);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
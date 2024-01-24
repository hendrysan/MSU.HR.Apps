using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using System;
using static Models.Entities.EnumEntities;

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
        public DbSet<MasterAttendance> MasterAttendances { get; set; }
        public DbSet<MasterShift> MasterShifts { get; set; }
        public DbSet<MasterWorkDay> MasterWorkDays { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //var con = _configuration.GetConnectionString("PostgreSQLConnection");

            var env = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            var val = env["MSU_HRIS_PostgreSQLConnection"]?.ToString();


            //var con = _configuration.GetConnectionString(val);
            optionsBuilder.UseNpgsql(val);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converterModul = new ValueConverter<EnumModule, string>(
                v => v.ToString(),
                 v => (EnumModule)Enum.Parse(typeof(EnumModule), v));

            var converterSource = new ValueConverter<EnumSource, string>(
                v => v.ToString(),
                 v => (EnumSource)Enum.Parse(typeof(EnumSource), v));

            modelBuilder
               .Entity<GrantAccess>()
               .Property(e => e.Module)
               .HasConversion(converterModul);

            modelBuilder
               .Entity<GrantAccess>()
               .Property(e => e.Source)
               .HasConversion(converterSource);
        }

    }
}
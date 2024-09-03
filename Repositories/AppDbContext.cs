using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rbac_IctJohor.Models;

namespace Rbac_IctJohor.Repositories
{
    public class AppDbContext:IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {

        public AppDbContext(DbContextOptions<AppDbContext>options)
            :base(options)
        {
            
        }

        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<AgencyGrouping> AgencyGroupings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Agencies)
            .WithOne(a => a.Tenant)
            .HasForeignKey(a => a.TenantId);

            modelBuilder.Entity<Agency>()
                .HasMany(a => a.Users)
                .WithOne(u => u.Agency)
                .HasForeignKey(u => u.AgencyId);

            modelBuilder.Entity<Agency>()
               .Property(e => e.AgencyGroup)
               .HasConversion(
               v => JsonConvert.SerializeObject(v),
               v => JsonConvert.DeserializeObject<List<string>>(v));
        }
    }

}

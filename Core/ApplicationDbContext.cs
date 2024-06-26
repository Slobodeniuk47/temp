using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ApplicationDbContext : IdentityDbContext
        <
           UserEntity, RoleEntity, long, IdentityUserClaim<long>,
           PermissionsEntity, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>
        >
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //new DbInitializer(modelBuilder).Seed();
            modelBuilder.Entity<PermissionsEntity>(permissions =>
            {
                permissions.HasKey(perm => new { perm.UserId, perm.RoleId });
                //Role
                permissions.HasOne(perm => perm.Role)
                   .WithMany(perm => perm.Permissions)
                   .HasForeignKey(perm => perm.RoleId)
                   .IsRequired();

                //User
                permissions.HasOne(perm => perm.User)
                   .WithMany(perm => perm.Permissions)
                   .HasForeignKey(perm => perm.UserId)
                   .IsRequired();
            });
        }
    }
}

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Core.DTOs.Identity;
using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DB
{
    public class AppDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>,ApplicationUserRole, IdentityUserLogin<int>,IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AppDBContext(DbContextOptions<AppDBContext> options, IHttpContextAccessor httpContextAccessor)
           : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public virtual DbSet<LookupItem> LookupItems { get; set; }
        public virtual DbSet<DataLog> DataLogs { get; set; }
        public virtual DbSet<Center> Centers { get; set; }
        public virtual DbSet<CenterImage> CenterImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.ApplicationUserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            //Seed Data
            var superAdminRole = new ApplicationRole()
            {
                Id = 1,
                Name = Core.CommonStaticFunctions.Role_SuperAdmin,
                NormalizedName = Core.CommonStaticFunctions.Role_SuperAdmin,
                CreatedBy = 0,
                CreationDate = DateTime.Now
            };
            builder.Entity<ApplicationRole>().HasData(superAdminRole);

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole()
            {
                Id = 2,
                Name = Core.CommonStaticFunctions.Role_Admin,
                NormalizedName = Core.CommonStaticFunctions.Role_Admin,
                CreatedBy = 0,
                CreationDate = DateTime.Now
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole()
            {
                Id = 3,
                Name = Core.CommonStaticFunctions.Role_Employee,
                NormalizedName = Core.CommonStaticFunctions.Role_Employee,
                CreatedBy = 0,
                CreationDate = DateTime.Now
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole()
            {
                Id = 4,
                Name = Core.CommonStaticFunctions.Role_Customer,
                NormalizedName = Core.CommonStaticFunctions.Role_Customer,
                CreatedBy = 0,
                CreationDate = DateTime.Now
            });

            //SuperAdmin User
            var superAdminUser = new ApplicationUser
            {
                Id = 1,
                FullName = "Super Admin User",
                UserName = "SuperAdmin@tetco.sa",
                NormalizedUserName = "SuperAdmin@tetco.sa",
                Email = "SuperAdmin@tetco.sa",
                NormalizedEmail = "SuperAdmin@tetco.sa",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(superAdminUser, "Admin1");
            superAdminUser.PasswordHash = hashed;

            builder.Entity<ApplicationUser>().HasData(superAdminUser);

            builder.Entity<ApplicationUserRole>().HasData(new ApplicationUserRole()
            {
                RoleId = superAdminRole.Id,
                UserId = superAdminUser.Id
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var changes = this.ChangeTracker.Entries().Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted).ToList();
            var dNow = DateTime.Now;
            try
            {
                foreach (var item in changes)
                {
                    BaseEntity baseEntity = null;
                    string userId = "0";
                    try
                    {
                        baseEntity = (BaseEntity)item.Entity;

                        userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    }
                    catch
                    {
                        //Do nothing
                        continue;
                    }

                    switch (item.State)
                    {
                        case EntityState.Added:

                            if (baseEntity != null)
                            {
                                baseEntity.IsDeleted = baseEntity.IsDeleted;
                                baseEntity.CreationDate = baseEntity.CreationDate == DateTime.MinValue ? dNow : baseEntity.CreationDate;
                                baseEntity.CreatedBy = int.Parse(userId);
                            }

                            break;

                        case EntityState.Modified:

                            if (baseEntity != null)
                            {
                                baseEntity.IsDeleted = baseEntity.IsDeleted;
                                baseEntity.LastUpdateDate = !baseEntity.LastUpdateDate.HasValue || baseEntity.LastUpdateDate == DateTime.MinValue ? dNow : baseEntity.LastUpdateDate;
                                baseEntity.LastUpdatedBy = int.Parse(userId);
                            }

                            try
                            {
                                DataLog entity = new DataLog();
                                entity.Transaction = item.State.ToString();
                                //entity.TableName = item.Entity.GetType().BaseType.Name.ToLower() != "object" ? item.Entity.GetType().BaseType.Name : item.Entity.GetType().Name;
                                entity.TableName = item.Entity.GetType().Name;
                                entity.CreationDate = dNow;
                                entity.UserId = int.Parse(userId);
                                entity.Data = JsonConvert.SerializeObject(new { OriginalValues = item.OriginalValues?.ToObject(), CurrentValues = item.CurrentValues?.ToObject() });
                                entity.RowId = (int)item.OriginalValues["Id"];
                                this.DataLogs.Add(entity);
                            }
                            catch (Exception ex)
                            {
                                //Do Nothing
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return await base.SaveChangesAsync();
        }
    }
}

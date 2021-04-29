using Core.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace DB
{
    public class SeedData
    {
        private DBContext context;

        public SeedData(DBContext context)
        {
            this.context = context;
        }

        public async void SeedAdminUsers()
        {
            try
            {
                //Add Roles
                var superAdminRole = new ApplicationRole()
                {
                    Name = Core.CommonStaticFunctions.Role_SuperAdmin,
                    CreatedBy = 0,
                    CreationDate = DateTime.Now
                };

                if (!context.Roles.Any(a => a.Name == Core.CommonStaticFunctions.Role_SuperAdmin))
                    await context.Roles.AddAsync(superAdminRole);

                if (!context.Roles.Any(a => a.Name == Core.CommonStaticFunctions.Role_Admin))
                    await context.Roles.AddAsync(new ApplicationRole()
                    {
                        Name = Core.CommonStaticFunctions.Role_Admin,
                        CreatedBy = 0,
                        CreationDate = DateTime.Now
                    });

                if (!context.Roles.Any(a => a.Name == Core.CommonStaticFunctions.Role_Employee))
                    await context.Roles.AddAsync(new ApplicationRole()
                    {
                        Name = Core.CommonStaticFunctions.Role_Employee,
                        CreatedBy = 0,
                        CreationDate = DateTime.Now
                    });

                if (!context.Roles.Any(a => a.Name == Core.CommonStaticFunctions.Role_Customer))
                    await context.Roles.AddAsync(new ApplicationRole()
                    {
                        Name = Core.CommonStaticFunctions.Role_Customer,
                        CreatedBy = 0,
                        CreationDate = DateTime.Now
                    });


                await context.SaveChangesAsync();

                //SuperAdminUser
                var superAdminUser = new ApplicationUser
                {
                    FullName = "Super Admin User",
                    UserName = "SuperAdmin",
                    NormalizedUserName = "SUPERADMIN",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                if (!context.Users.Any(u => u.UserName == superAdminUser.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(superAdminUser, "Admin1");
                    superAdminUser.PasswordHash = hashed;

                    await context.Users.AddAsync(superAdminUser);

                    await context.UserRoles.AddAsync(new ApplicationUserRole()
                    {
                        RoleId = superAdminRole.Id,
                        UserId = superAdminUser.Id
                    });
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await context.DisposeAsync();
            }
        }
    }
}

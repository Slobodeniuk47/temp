using Core.Constants;
using Core.Entities.Identity;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi
{
    public static class DbSeeder
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                
                if (!roleManager.Roles.Any())
                {
                    foreach (var role in Roles.All)
                    {
                        var result = roleManager.CreateAsync(new RoleEntity
                        {
                            Name = role
                        }).Result;
                    }
                }
                if (!userManager.Users.Any())
                {
                    var user = new UserEntity
                    {
                        Email = "admin@gmail.com",
                        FirstName = "Admin",
                        LastName = "Ludomanskiy",
                        Image = "admin.jpg",
                        PhoneNumber = "+380 77 777 77 77"
                    };
                    var result = userManager.CreateAsync(user, "123456").Result;
                    var role = Roles.Admin.FirstOrDefault();
                    result = userManager.AddToRoleAsync(user, Roles.Admin).Result;
                }
            }
        }
    }
}

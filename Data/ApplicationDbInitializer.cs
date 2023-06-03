using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Identity;


namespace DigitalMarketing2.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                // Seed Modules
                if (!context.Module.Any())
                {
                context.Module.AddRange(new List<Module>()
                {
                    new Module()
                    {
                        Name = "Take a business online",
                        Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
                        ModuleOrder = 1,
                    },
                    new Module()
                    {
                        Name = "Make it easy for people to find a business on the web",
                        Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
                        ModuleOrder = 2,
                    },
                    new Module()
                    {
                        Name = "Take a Business Online",
                        Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
                        ModuleOrder = 3,
                    },
                    new Module()
                    {
                        Name = "Take a Business Online",
                        Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
                        ModuleOrder = 4,
                    },
                    new Module()
                    {
                        Name = "Take a Business Online",
                        Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
                        ModuleOrder = 5,
                    },
                });
                context.SaveChanges();
                }
            
                // Seed Lessons

                // Seed LessonSections

                // Seed Quiz Questions

                // Seed Question Options

                // Seed Student Score

                // Seed Discussions
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                // Seeding Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("Registered"))
                    await roleManager.CreateAsync(new IdentityRole("Registered"));

                // Seeding Admin
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminEmail = "jiayong1008@marketup.com";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if(adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "limjiayong",
                        Email = "jiayong1008@marketup.com",
                        Gender = Gender.female,
                    };
                    await userManager.CreateAsync(newAdminUser, "Qwerty@123");
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }

                // Seed Registered Users
                string appUserEmail = "april@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new User()
                    {
                        UserName = "apriltan",
                        Email = appUserEmail,
                        Gender = Gender.female,
                    };
                    await userManager.CreateAsync(newAppUser, "Qwerty@123");
                    await userManager.AddToRoleAsync(newAppUser, "Registered");
                }
            }
        }


    }
}

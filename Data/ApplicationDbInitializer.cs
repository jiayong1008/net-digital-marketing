using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DigitalMarketing2.Data
{
    public class AppDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                // Seed Modules
                if (!context.Module.Any())
                    SeedModule(context);
            
                // Seed Lessons
                if (!context.Lesson.Any())
                {
                    // Seed module 1 lessons
                    var module1 = await context.Module.FirstOrDefaultAsync(); // Get the first module from the database
                    if (module1 != null)
                    {
                        context.Lesson.AddRange(new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Name = "Build your online shop",
                                Duration = 25,
                                LessonOrder = 1,
                                ModuleId = module1.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Sell more online",
                                Duration = 35,
                                LessonOrder = 2,
                                ModuleId = module1.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Build your web presence",
                                Duration = 40,
                                LessonOrder = 3,
                                ModuleId = module1.ModuleId,
                            }
                        });
                        context.SaveChanges();
                    }

                    // Seed module 2 lessons
                    var module2 = await context.Module.FirstOrDefaultAsync(m => m.ModuleOrder == 2); // Get the first module from the database
                    if (module2 != null)
                    {
                        context.Lesson.AddRange(new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Name = "Help people nearby find you online",
                                Duration = 25,
                                LessonOrder = 1,
                                ModuleId = module2.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Get start with search",
                                Duration = 45,
                                LessonOrder = 2,
                                ModuleId = module2.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Make search work for you",
                                Duration = 25,
                                LessonOrder = 3,
                                ModuleId = module2.ModuleId,
                            }
                        });
                        context.SaveChanges();
                    }

                    // Seed module 5 lessons
                    var module3 = await context.Module.FirstOrDefaultAsync(m => m.ModuleOrder == 3); // Get the first module from the database
                    if (module3 != null)
                    {
                        context.Lesson.AddRange(new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Name = "Make mobile work for you",
                                Duration = 35,
                                LessonOrder = 1,
                                ModuleId = module3.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Discover the possibilities of mobile",
                                Duration = 20,
                                LessonOrder = 2,
                                ModuleId = module3.ModuleId,
                            }
                        });
                        context.SaveChanges();
                    }

                    // Seed module 4 lessons
                    var module4 = await context.Module.FirstOrDefaultAsync(m => m.ModuleOrder == 4); // Get the first module from the database
                    if (module4 != null)
                    {
                        context.Lesson.AddRange(new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Name = "Expand internationally",
                                Duration = 70,
                                LessonOrder = 1,
                                ModuleId = module4.ModuleId,
                            }
                        });
                        context.SaveChanges();
                    }

                    // Seed module 5 lessons
                    var module5 = await context.Module.FirstOrDefaultAsync(m => m.ModuleOrder == 5); // Get the first module from the database
                    if (module5 != null)
                    {
                        context.Lesson.AddRange(new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Name = "Advertise on other websites",
                                Duration = 25,
                                LessonOrder = 1,
                                ModuleId = module5.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Be noticed with search ads",
                                Duration = 25,
                                LessonOrder = 2,
                                ModuleId = module5.ModuleId,
                            },
                            new Lesson()
                            {
                                Name = "Improve your search campaigns",
                                Duration = 30,
                                LessonOrder = 3,
                                ModuleId = module5.ModuleId,
                            }
                        });
                        context.SaveChanges();
                    }
                }

                // Seed LessonSections


                // Seed Quiz Questions

                // Seed Question Options

                // Seed Student Score

                // Seed Discussions
            }
        }

        public static async void SeedModule(ApplicationDbContext context)
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
                    Description = "Start our free course to discover just some of the ways businesses can reach and connect with more customers online. Plus, learn how to improve your search engine performance (SEO), and use online advertising (SEM) to boost sales and awareness.",
                    ModuleOrder = 2,
                },
                new Module()
                {
                    Name = "Get yourself noticed on social media",
                    Description = "Everyone is on social media, so it makes sense that your business should be on social media too. To take advantage of popular social media networks, understand why you need to be there, join the right social media sites, and increase your presence by engaging with them.",
                    ModuleOrder = 3,
                },
                new Module()
                {
                    Name = "Expand a business to other countries",
                    Description = "Learn how to take your business global and start selling to customers abroad.",
                    ModuleOrder = 4,
                },
                new Module()
                {
                    Name = "Promote a business with online advertising",
                    Description = "Discover the online tools you can use to promote a business online, create your marketing strategy, and attract the right customers.",
                    ModuleOrder = 5,
                }
            });
            context.SaveChanges();
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

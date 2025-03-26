using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models;
using EcommerceRestApi.Helpers.Static;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommerceRestApi.Helpers.Data.ViewModels;

namespace EcommerceRestApi.Helpers.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                // Seed Categories
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>
            {
                new Category { Code = "CAT001", Name = "Electronics", About = "Electronic items", IsActive = true, DateCreated = DateTime.Now },
                new Category { Code = "CAT002", Name = "Fashion", About = "Clothing and accessories", IsActive = true, DateCreated = DateTime.Now }
            });
                    context.SaveChanges();
                }

                // Seed Subcategories
                if (!context.Subcategories.Any())
                {
                    context.Subcategories.AddRange(new List<Subcategory>
            {
                new Subcategory { Code = "SCAT001", Name = "Mobile Phones", About = "Smartphones and feature phones", CategoryId = context.Categories.First().Id, IsActive = true, DateCreated = DateTime.Now },
                new Subcategory { Code = "SCAT002", Name = "Men's Wear", About = "Clothing for men", CategoryId = context.Categories.ToList()[1].Id, IsActive = true, DateCreated = DateTime.Now }
            });
                    context.SaveChanges();
                }

                // Seed Countries
                if (!context.Countries.Any())
                {
                    context.Countries.AddRange(new List<Country>
            {
                new Country { CountryName = "United States", CountryCode = "US", IsActive = true, DateCreated = DateTime.Now },
                new Country { CountryName = "Canada", CountryCode = "CA", IsActive = true, DateCreated = DateTime.Now }
            });
                    context.SaveChanges();
                }

                // Seed Delivery Methods
                if (!context.DeliveryMethods.Any())
                {
                    context.DeliveryMethods.AddRange(new List<DeliveryMethod>
            {
                new DeliveryMethod { MethodName = "Standard Shipping", IsActive = true, DateCreated = DateTime.Now },
                new DeliveryMethod { MethodName = "Express Shipping", IsActive = true, DateCreated = DateTime.Now }
            });
                    context.SaveChanges();
                }

                // Seed Payment Methods
                if (!context.PaymentMethods.Any())
                {
                    context.PaymentMethods.AddRange(new List<PaymentMethod>
            {
                new PaymentMethod { PaymentType = "Credit Card", Details = "Visa, MasterCard, Amex", IsActive = true, DateCreated = DateTime.Now },
                new PaymentMethod { PaymentType = "PayPal", Details = "Secure online payments", IsActive = true, DateCreated = DateTime.Now }
            });
                    context.SaveChanges();
                }

                // Seed Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>
            {
                new Product { Name = "iPhone 13", Brand = "Apple", Code = "IP13", Price = 999, Stock = 10, SubcategoryId = context.Subcategories.First().Id, LongAbout = "Latest iPhone model", Photo = "https://tinyurl.com/2p8ypn72",
                    OtherPhotos = "https://tinyurl.com/2p8ypn72", IsActive = true, DateCreated = DateTime.Now },
                new Product { Name = "Nike Sneakers", Brand = "Nike", Code = "NS001", Price = 120, Stock = 25, SubcategoryId = context.Subcategories.ToList().First().Id, LongAbout = "High-quality running shoes",
                    Photo = "https://tinyurl.com/2p8ypn72", 
                    OtherPhotos = "https://tinyurl.com/2p8ypn72", IsActive = true, DateCreated = DateTime.Now },

            });
                    context.SaveChanges();
                }
            }
        }


        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                //Roles Section
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //user
                var UserManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "admin@etickets.com";
                var adminUser = await UserManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        Role = UserRoles.Admin,
                        IsAdmin = true,
                        IsActive = true,
                        EmailConfirmed = true,
                        DateCreated = DateTime.Now
                    };
                    await UserManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await UserManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);

                }
                string appUserEmail = "user@etickets.com";
                var appUser = await UserManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FullName = "Application User",
                        UserName = "user",
                        Email = appUserEmail,
                        Role = UserRoles.User,
                        IsAdmin = false,
                        IsActive = true,
                        EmailConfirmed = true,
                        DateCreated = DateTime.Now
                    };

                    newAppUser.Customers.Add(
                        new Customer()
                        {
                            IsActive = true,
                            DateCreated = DateTime.Now,
                            Nip = "12345678910",
                            UserId = newAppUser.Id,
                            Points = 0,
                        }
                    );

                    await UserManager.CreateAsync(newAppUser, "Coding@1234?");
                    await UserManager.AddToRoleAsync(newAppUser, UserRoles.User);

                }
            }
        }

    }
}

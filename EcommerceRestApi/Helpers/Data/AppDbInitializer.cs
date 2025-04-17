using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Static;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Identity;

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
                    if (context.Categories.ToList().Count >= 2)
                    {
                        context.Subcategories.AddRange(new List<Subcategory>
                        {
                            new Subcategory { Code = "SCAT001", Name = "Mobile Phones", About = "Smartphones and feature phones", CategoryId = context.Categories.First().Id, IsActive = true, DateCreated = DateTime.Now },
                            new Subcategory { Code = "SCAT002", Name = "Men's Wear", About = "Clothing for men", CategoryId = context.Categories.ToList()[1].Id, IsActive = true, DateCreated = DateTime.Now }
                        });
                        context.SaveChanges();

                    }
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
                        new DeliveryMethod { MethodName = OrderProcessingFuncs.GetStringValue(Enums.DeliveryMethods.Delivery), IsActive = true, DateCreated = DateTime.Now },
                        new DeliveryMethod { MethodName = OrderProcessingFuncs.GetStringValue(Enums.DeliveryMethods.TakeAway), IsActive = true, DateCreated = DateTime.Now },
                        new DeliveryMethod { MethodName = OrderProcessingFuncs.GetStringValue(Enums.DeliveryMethods.ParcelLocker), IsActive = true, DateCreated = DateTime.Now },
                        new DeliveryMethod { MethodName = OrderProcessingFuncs.GetStringValue(Enums.DeliveryMethods.Courier), IsActive = true, DateCreated = DateTime.Now },

                    });
                    context.SaveChanges();
                }

                // Seed Payment Methods
                if (!context.PaymentMethods.Any())
                {
                    context.PaymentMethods.AddRange(new List<PaymentMethod>
                    {
                        new PaymentMethod { PaymentType = OrderProcessingFuncs.GetStringValue(Enums.PaymentMethods.Card), Details = "Visa, MasterCard, Amex", IsActive = true, DateCreated = DateTime.Now },
                        new PaymentMethod { PaymentType =  OrderProcessingFuncs.GetStringValue(Enums.PaymentMethods.PayPal), Details = "Secure online payments", IsActive = true, DateCreated = DateTime.Now },
                        new PaymentMethod { PaymentType =  OrderProcessingFuncs.GetStringValue(Enums.PaymentMethods.Transaction), Details = "Bank transfers", IsActive = true, DateCreated = DateTime.Now },
                        new PaymentMethod { PaymentType =  OrderProcessingFuncs.GetStringValue(Enums.PaymentMethods.Cash), Details = "Pay upon getting a parcel deliverred to you or at takeaway", IsActive = true, DateCreated = DateTime.Now }
                    });
                    context.SaveChanges();
                }
                // Seed Products
                if (!context.Products.Any())
                {
                    if (context.Subcategories.ToList().Count > 0)
                    {
                        var products = new List<Product>
                            {
                                new Product { Name = "iPhone 13", Brand = "Apple", Code = "IP13", Price = 999, Stock = 10,
                                                SubcategoryId = context.Subcategories.First().Id, LongAbout = "Latest iPhone model",
                                                Photo = "https://tinyurl.com/2p8ypn72", OtherPhotos = "https://tinyurl.com/2p8ypn72",
                                                IsActive = true, DateCreated = DateTime.Now },
                                new Product { Name = "Nike Sneakers", Brand = "Nike", Code = "NS001",
                                            Price = 120, Stock = 25, SubcategoryId = context.Subcategories.ToList().First().Id,
                                            LongAbout = "High-quality running shoes", Photo = "https://t.ly/y0Eky",
                                            OtherPhotos = "https://t.ly/y0Eky", IsActive = true, DateCreated = DateTime.Now },
                                new Product { Name = "Nike Sneakers", Brand = "Nike", Code = "NS002", Price = 120, Stock = 25,
                                              SubcategoryId = context.Subcategories.ToList().First().Id,
                                              LongAbout = "High-quality running shoes2", Photo = "https://t.ly/y0Eky",
                                              OtherPhotos = "https://t.ly/y0Eky", IsActive = true, DateCreated = DateTime.Now },

                        };

                        products.ForEach(p =>
                        {
                            if (p.Code == products[0].Code)
                            {
                                p.ProductCategories.Add(new ProductCategory
                                {
                                    CategoryId = context.Categories.First().Id,
                                    IsActive = true,
                                    DateCreated = DateTime.Now
                                });
                            }
                            else
                            {
                                p.ProductCategories.Add(new ProductCategory
                                {
                                    CategoryId = context.Categories.ToArray()[1].Id,
                                    IsActive = true,
                                    DateCreated = DateTime.Now
                                });
                            }
                            p.OldPrice = p.Price;
                        });

                        context.Products.AddRange();


                        context.SaveChanges();
                    }
                }

                if (!context.Orders.Any())
                {
                    if (context.Customers.Any() && context.Customers.ToList().Count >= 3)
                    {
                        context.Orders.AddRange(new List<Order>
                        {
                            new Order { Code = Guid.NewGuid().ToString(),  Status = OrderProcessingFuncs.GetStringValue(Enums.OrderStatuses.Approved), IsActive = true,  CustomerId = context.Customers.First().Id, TotalAmount = 0, OrderDate = DateTime.Now, DateCreated = DateTime.Now },
                            new Order { Code = Guid.NewGuid().ToString(), Status = OrderProcessingFuncs.GetStringValue(Enums.OrderStatuses.Approved), IsActive = true, CustomerId = context.Customers.ToArray()[2].Id, TotalAmount = 0, OrderDate = DateTime.Now, DateCreated = DateTime.Now },
                            new Order { Code = Guid.NewGuid().ToString(), Status = OrderProcessingFuncs.GetStringValue(Enums.OrderStatuses.Approved), IsActive = true, CustomerId = context.Customers.ToArray()[2].Id, TotalAmount = 0, OrderDate = DateTime.Now, DateCreated = DateTime.Now },

                        });
                        context.SaveChanges();

                    }
                }
                if (!context.OrderItems.Any())
                {
                    if (context.Products.Any() && context.Orders.Any() && context.Products.ToList().Count >= 3 && context.Orders.ToList().Count >= 3)
                    {
                        context.OrderItems.AddRange(new List<OrderItem>
                                {
                                    new OrderItem { OrderId = context.Orders.First().Id, IsActive = true, ProductId = context.Products.First().Id, Quantity = 2, UnitPrice = 12, DateCreated = DateTime.Now },
                                    new OrderItem { OrderId = context.Orders.ToArray()[1].Id, IsActive = true, ProductId = context.Products.ToArray()[1].Id, Quantity = 2, UnitPrice = 12, DateCreated = DateTime.Now },
                                    new OrderItem { OrderId = context.Orders.ToArray()[2].Id, IsActive = true, ProductId = context.Products.ToArray()[2].Id, Quantity = 2, UnitPrice = 12, DateCreated = DateTime.Now },

                                });


                        context.SaveChanges();

                        foreach (var item in context.OrderItems)
                        {
                            var order = context.Orders.FirstOrDefault(o => o.Id == item.OrderId);
                            if (order != null)
                            {
                                order.TotalAmount += item.Quantity * item.UnitPrice;
                            }
                        }

                        context.SaveChanges();
                    }
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

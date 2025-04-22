using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;

namespace EcommerceRestApi.Helpers.Data.Functions
{
    public class DbFuncs
    {
        // Reviews 
        public static void UpdateRatingSum(Product product)
        {
            if (product.Reviews == null || !product.Reviews.Any()) return;

            product.RatingSum = product.Reviews.Sum(r => r.Rating);
        }

        public static void UpdateRatingVotesCount(Product product)
        {
            if (product.Reviews == null || !product.Reviews.Any()) return;

            product.RatingVotes = product.Reviews.Count;
        }

        public static List<CountryViewModel> GetCountriesList(AppDbContext context)
        {
            CountryService _countryService = new CountryService(context);
            return _countryService.GetCountriesList();
        }
        //Categories

        public static Category FindCategoryById(int id)
        {
            using (var db = new AppDbContext())
            {
                var category = (from c in db.Categories
                                where c.Id == id
                                select c).FirstOrDefault();
                return category;
            }
        }
        public static void AddCategory(Category category)
        {
            if (category == null)
                return;
            using (var db = new AppDbContext())
            {
                db.Categories.Add(category);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateCategory(Category updatedCategory)
        {
            using (var db = new AppDbContext())
            {
                var category = (from c in db.Categories
                                where c.Id == updatedCategory.Id
                                select c).FirstOrDefault();
                if (category == null)
                    return;
                category.Name = updatedCategory.Name;
                category.Code = updatedCategory.Code;
                category.About = updatedCategory.About;
                category.IsActive = updatedCategory.IsActive;


                category.DateUpdated = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        //Subcategories


        public static Subcategory FindSubcategoryById(int id)
        {
            using (var db = new AppDbContext())
            {
                var subcategory = (from c in db.Subcategories
                                   where c.Id == id
                                   select c).FirstOrDefault();
                return subcategory;
            }
        }
        public static void AddSubcategory(Subcategory subcategory)
        {
            if (subcategory == null)
                return;
            using (var db = new AppDbContext())
            {
                db.Subcategories.Add(subcategory);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateSubategory(Subcategory updatedSubcategory)
        {
            using (var db = new AppDbContext())
            {
                var subcategory = (from c in db.Subcategories
                                   where c.Id == updatedSubcategory.Id
                                   select c).FirstOrDefault();
                if (subcategory == null)
                    return;
                subcategory.Name = updatedSubcategory.Name;
                subcategory.Code = updatedSubcategory.Code;
                subcategory.About = updatedSubcategory.About;
                subcategory.CategoryId = updatedSubcategory.CategoryId;
                subcategory.IsActive = updatedSubcategory.IsActive;

                subcategory.DateUpdated = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        //Products


        public static Product FindProductById(int id)
        {
            using (var db = new AppDbContext())
            {
                var product = (from c in db.Products
                               where c.Id == id
                               select c).FirstOrDefault();
                return product;
            }
        }


        //Users

        public async static Task<ApplicationUser> GetApplicationUserObjForRegister(RegisterViewModel registerVM, AppDbContext _context)
        {
            ApplicationUser newUser = new ApplicationUser()
            {
                FullName = registerVM.FirstName + " " + registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.Username ?? registerVM.Email.Split("@")[0],
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Role = registerVM.IsAdmin ? UserRoles.Admin : UserRoles.User,
                PhoneNumber = registerVM.PhoneNumber,
                IsActive = registerVM.IsActive,
                IsAdmin = registerVM.IsAdmin,
                DateCreated = DateTime.Now
            };

            var newCustomer = new Customer()
            {
                IsActive = newUser.IsActive,
                DateCreated = DateTime.UtcNow,
                Nip = registerVM.Nip,
                Points = 0,
                UserId = newUser.Id
            };

            var country = DbFuncs.GetCountriesList(_context).FirstOrDefault(c => c.CountryName.ToLower() == registerVM.CountryName?.ToLower());
            newCustomer.Addresses.Add(new Address()
            {
                City = registerVM.City,
                CountryId = country != null ? country.Id : null,
                PostalCode = registerVM.PostalCode,
                FlatNumber = registerVM.FlatNumber,
                HouseNumber = registerVM.HouseNumber,
                Street = registerVM.Street,
                State = registerVM.State,
                CustomerId = newCustomer.Id,
                IsActive = newUser.IsActive,
            });


            newUser.Customers.Add(newCustomer);

            return newUser;
        }


        public static ApplicationUser FindUserById(string id)
        {
            using (var db = new AppDbContext())
            {
                var user = (from c in db.Users
                            where c.Id.Equals(id)
                            select c).FirstOrDefault();
                return user;
            }
        }
        public static ApplicationUser FindUserByEmail(string email)
        {
            using (var db = new AppDbContext())
            {
                var user = (from c in db.Users
                            where c.Email == email
                            select c).FirstOrDefault();
                return user;
            }
        }

        // Orders

        public static Order FindOrderById(int id)
        {
            using (var db = new AppDbContext())
            {
                var order = (from o in db.Orders
                             where o.Id == id
                             select o).FirstOrDefault();
                return order;
            }
        }
        public static Order FindOrderByCode(string code)
        {
            using (var db = new AppDbContext())
            {
                var order = (from o in db.Orders
                             where o.Code == code
                             select o).FirstOrDefault();
                return order;
            }
        }
        public static void UpdateOrder(Order updatedOrder)
        {
            using (var db = new AppDbContext())
            {
                var order = (from o in db.Orders
                             where o.Id == updatedOrder.Id
                             select o).FirstOrDefault();
                if (order == null)
                    return;
                order.Code = updatedOrder.Code;
                order.TotalAmount = updatedOrder.TotalAmount;
                order.Payments = updatedOrder.Payments;
                order.Status = updatedOrder.Status;
                order.Customer = updatedOrder.Customer;
                order.CustomerId = updatedOrder.CustomerId;
                order.OrderDate = updatedOrder.OrderDate;

                order.DeliveryMethodOrders = updatedOrder.DeliveryMethodOrders;
                order.OrderItems = updatedOrder.OrderItems;
                order.Shipments = updatedOrder.Shipments;
                order.DateUpdated = DateTime.UtcNow;

                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }
    }
}

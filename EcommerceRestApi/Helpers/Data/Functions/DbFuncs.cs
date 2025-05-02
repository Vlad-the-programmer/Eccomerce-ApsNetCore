using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
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
            product.DateUpdated = DateTime.Now;
        }

        public static void UpdateRatingVotesCount(Product product)
        {
            if (product.Reviews == null || !product.Reviews.Any()) return;

            product.RatingVotes = product.Reviews.Count;
            product.DateUpdated = DateTime.Now;
        }

        public static List<CountryDTO> GetCountriesList(AppDbContext context)
        {
            CountryService _countryService = new CountryService(context);
            return _countryService.GetCountriesList();
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
    }
}

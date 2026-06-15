using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class ReviewService : EntityBaseRepository<Review>, IReviewsService
    {
        private readonly AppDbContext _context;
        public ReviewService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ReviewDto> AddNewReviewAsync(ReviewCreateEditVM data)
        {
            var review = new Review
            {
                CustomerId = data.CustomerId,
                ProductId = data.ProductId,
                Rating = data.Rating,
                ReviewText = data.ReviewText,
                IsActive = true,
                DateCreated = DateTime.Now,
            };


            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            var newReview = await _context.Reviews
                                              .Include(r => r.Product)
                                              .Include(r => r.Customer)
                                                .ThenInclude(c => c.User)
                                              .FirstOrDefaultAsync(r => r.Id == review.Id);
            DbFuncs.UpdateRatingSum(newReview.Product);
            DbFuncs.UpdateRatingVotesCount(newReview.Product);

            await _context.SaveChangesAsync();
            return ReviewDto.FromEntity(newReview, false);
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews
                                              .Include(r => r.Product)
                                              .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return false;

            review.IsActive = false;

            DbFuncs.UpdateRatingSum(review.Product);
            DbFuncs.UpdateRatingVotesCount(review.Product);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReviewDto?> GetReviewByIDAsync(int id)
        {
            return await _context.Reviews
                .Where(r => r.Id == id && r.IsActive)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    IsActive = r.IsActive,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ProductId = r.ProductId,
                    CustomerId = r.CustomerId,
                    UserName = r.Customer != null && r.Customer.User != null ? r.Customer.User.UserName : null,
                    Customer = r.Customer != null ? new CustomerDto
                    {
                        Id = r.Customer.Id,
                        CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                        FullName = r.Customer.User != null ? r.Customer.User.FullName : string.Empty,
                        Email = r.Customer.User != null ? r.Customer.User.Email : null,
                        Nip = r.Customer.Nip,
                        Points = r.Customer.Points,
                        IsActive = r.Customer.IsActive,
                        DateCreated = r.Customer.DateCreated,
                        Address = r.Customer.Addresses.FirstOrDefault() != null ? new AddressDto
                        {
                            Id = r.Customer.Addresses.FirstOrDefault().Id,
                            CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                            CustomerId = r.Customer.Addresses.FirstOrDefault().CustomerId,
                            Street = r.Customer.Addresses.FirstOrDefault().Street,
                            HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                            FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                            City = r.Customer.Addresses.FirstOrDefault().City,
                            State = r.Customer.Addresses.FirstOrDefault().State,
                            PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode
                        } : null
                    } : null!,
                    Product = null!,
                    DateCreated = r.DateCreated,
                    DateUpdated = r.DateUpdated ?? DateTime.MinValue,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReviewDto>> GetReviews()
        {
            return await _context.Reviews
                                .Where(r => r.IsActive)
                                .Select(r => new ReviewDto
                                {
                                    Id = r.Id,
                                    IsActive = r.IsActive,
                                    Rating = r.Rating,
                                    ReviewText = r.ReviewText,
                                    ProductId = r.ProductId,
                                    CustomerId = r.CustomerId,
                                    UserName = r.Customer != null && r.Customer.User != null ? r.Customer.User.UserName : null,
                                    Customer = r.Customer != null ? new CustomerDto
                                    {
                                        Id = r.Customer.Id,
                                        CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                                        FullName = r.Customer.User != null ? r.Customer.User.FullName : string.Empty,
                                        Email = r.Customer.User != null ? r.Customer.User.Email : null,
                                        Nip = r.Customer.Nip,
                                        Points = r.Customer.Points,
                                        IsActive = r.Customer.IsActive,
                                        DateCreated = r.Customer.DateCreated,
                                        Address = r.Customer.Addresses.FirstOrDefault() != null ? new AddressDto
                                        {
                                            Id = r.Customer.Addresses.FirstOrDefault().Id,
                                            CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                                            CustomerId = r.Customer.Addresses.FirstOrDefault().CustomerId,
                                            Street = r.Customer.Addresses.FirstOrDefault().Street,
                                            HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                                            FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                                            City = r.Customer.Addresses.FirstOrDefault().City,
                                            State = r.Customer.Addresses.FirstOrDefault().State,
                                            PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode
                                        } : null
                                    } : null!,
                                    Product = null!,
                                    DateCreated = r.DateCreated,
                                    DateUpdated = r.DateUpdated ?? DateTime.MinValue,
                                })
                                .ToListAsync();
        }

        public async Task<bool> UpdateReviewAsync(int id, ReviewUpdateViewModel data)
        {
            var review = _context.Reviews
                                    .Include(R => R.Product)
                                    .FirstOrDefault(r => r.Id == id);
            if (review == null) return false;

            await _context.Entry(review.Product)
            .Collection(p => p.Reviews)
            .LoadAsync();

            review.Rating = data.Rating;
            review.ReviewText = data.ReviewText;
            review.DateUpdated = DateTime.Now;

            DbFuncs.UpdateRatingSum(review.Product);
            DbFuncs.UpdateRatingVotesCount(review.Product);

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

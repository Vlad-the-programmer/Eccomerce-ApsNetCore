using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
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

        public async Task AddNewReviewAsync(ReviewViewModel data)
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

            data.Id = review.Id; // Set id of model for PostReview method to redirect user to GetReview

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDto?> GetReviewByIDAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return null;

            //return new ReviewViewModel
            //{
            //    Id = review.Id,
            //    Rating = review.Rating,
            //    ReviewText = review.ReviewText,
            //    Customer = review.Customer,
            //    CustomerId = review.CustomerId,
            //    ProductId = review.ProductId,
            //    DateCreated = review.DateCreated,
            //    DateUpdated = review.DateUpdated,
            //};
            return (ReviewDto)review;
        }

        public async Task<List<ReviewDto>> GetReviews()
        {
            return await _context.Reviews
                                .Include(r => r.Customer)
                                .Where(r => r.IsActive)
                                .Select(r => (ReviewDto)r)
                                .ToListAsync();
        }

        public async Task UpdateReviewAsync(int id, ReviewUpdateViewModel data)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                review.Rating = data.Rating;
                review.ReviewText = data.ReviewText;
                review.DateUpdated = DateTime.Now;
            }
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}

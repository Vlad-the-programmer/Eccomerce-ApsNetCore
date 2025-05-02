using EcommerceRestApi.Helpers.Data.Functions;
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
            review.Product.RatingSum -= review.Rating;
            review.Product.RatingVotes -= 1;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReviewDto?> GetReviewByIDAsync(int id)
        {
            var review = await _context.Reviews
                                .Include(r => r.Customer)
                                .ThenInclude(c => c.User)
                                //.Where(r => r.IsActive)
                                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return null;

            return ReviewDto.FromEntity(review);
        }

        public async Task<List<ReviewDto>> GetReviews()
        {
            return await _context.Reviews
                                .Include(r => r.Customer)
                                .ThenInclude(c => c.User)
                                .Where(r => r.IsActive)
                                .Select(r => ReviewDto.FromEntity(r, true))
                                .ToListAsync();
        }

        public async Task<bool> UpdateReviewAsync(int id, ReviewUpdateViewModel data)
        {
            var review = _context.Reviews
                                    .Include(R => R.Product)
                                    .FirstOrDefault(r => r.Id == id);
            if (review == null) return false;

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

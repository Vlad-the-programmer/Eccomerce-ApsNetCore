using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;

namespace EcommerceRestApi.Services
{
    public interface IReviewsService : IEntityBaseRepository<Review>
    {
        Task<ReviewDto> GetReviewByIDAsync(int id);
        Task<List<ReviewDto>> GetReviews();

        Task AddNewReviewAsync(ReviewViewModel data);

        Task<bool> UpdateReviewAsync(int id, ReviewUpdateViewModel data);
        Task<bool> DeleteReviewAsync(int id);
    }
}

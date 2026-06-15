using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;

namespace EcommerceRestApi.Services
{
    public interface IUserService
    {
        public Task DeleteUserAsync(string id);

        public Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        public Task<ApplicationUser> GetUserByIDAsync(string id);

        public Task UpdateUserAsync(string id, UserUpdateVM data);
    }
}

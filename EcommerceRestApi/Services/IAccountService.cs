using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;

namespace EcommerceRestApi.Services
{
    public interface IAccountService
    {
        Task<LoginResponse> Login(LoginViewModel loginVM, bool isMobileApp);
        Task<ResponseModel> Register(RegisterViewModel registerVM);
        Task Logout();
        Task<UserUpdateVM> GetUpdateUserModel(ApplicationUser user);
    }
}

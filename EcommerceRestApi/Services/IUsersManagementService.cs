using EcommerceRestApi.Helpers.Data.AuthVms;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using static EcommerceRestApi.Controllers.UsersManagementController;

namespace EcommerceRestApi.Services
{
    public interface IUsersManagementService
    {
        Task BulkUpdatePerms(UpdatePermissionsDto dto, ApplicationUser user);
        Task<IList<string>> GetPermissions(ApplicationUser user);
        Task<StaffUpdateVM> GetUpdateUserModel(ApplicationUser user);
        Task<List<CurrentUserViewModel>> GetStaffUsers();
        Task DeleteStaffUser(ApplicationUser user);
        Task<ResponseModel> CreateStaff(CreateStaffVM newUser);
        Task UpdateStaffAsync(ApplicationUser user, StaffUpdateVM userUpdateVM, string currentUserRole);
        Task<List<CurrentUserViewModel>> FilterStaffAsync(string searchString, string? searchProperty,
           string? sortProperty, string statusFilter, bool sortAscending = false);

        public List<SearchComboBoxDto> GetSearchComboBoxDtos();

        public List<SearchComboBoxDto> GetOrderByComboBoxDtos();
    }
}

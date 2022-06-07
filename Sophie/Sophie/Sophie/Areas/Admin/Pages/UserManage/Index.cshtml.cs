using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Core.Entities;
using App.Core.Constants;
using App.SharedLib.Repository.Interface;
using Sophie.Units;
using App.Core.Units;

namespace Sophie.Areas.Admin.Pages.UserManage
{
    [Authorize(Roles = RolePrefix.Developer)]
    //[Authorize(Roles = RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Moderator)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public partial class UserManageModel : PageModel
    {
        private readonly IAuthRepository _authRepo;
        private readonly IDevicesRepository _deviceRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<ApplicationRole> ListRole { get; set; }

        [BindProperty]
        public List<object> ListProvider { get; set; }

        public UserManageModel(IAuthRepository authRepo, IDevicesRepository deviceRepo, UserManager<ApplicationUser> userManager)
        {
            _authRepo = authRepo;
            _deviceRepo = deviceRepo;
            _userManager = userManager;
        }

        public void OnGet()
        {
            List<ApplicationRole> listRoles = _authRepo.ListRole();
            ListRole = listRoles;
            Logs.debug($"List roles {listRoles.Count} : \n {Newtonsoft.Json.JsonConvert.SerializeObject(listRoles, JsonSettings.SettingForNewtonsoft)}");

            List<object> listProvider = new List<object>();
            listProvider.Add(new { name_type = TypeProvider.Website.ToString() });
            listProvider.Add(new { name_type = TypeProvider.Google.ToString() });
            listProvider.Add(new { name_type = TypeProvider.Facebook.ToString() });
            listProvider.Add(new { name_type = TypeProvider.Twitter.ToString() });
            listProvider.Add(new { name_type = TypeProvider.Microsoft.ToString() });
            listProvider.Add(new { name_type = TypeProvider.Other.ToString() });
            ListProvider = listProvider;
            Logs.debug($"List provider {listProvider.Count} : \n {Newtonsoft.Json.JsonConvert.SerializeObject(listProvider, JsonSettings.SettingForNewtonsoft)}");
        }

        // Get list user
        public class PageFilter {
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }
        public Task<JsonResult> OnPostListItem(PageFilter filter)
        {
            try
            {
                Logs.debug($"ListItem filter: {Newtonsoft.Json.JsonConvert.SerializeObject(filter, JsonSettings.SettingForNewtonsoft)}");
                List<ApplicationUser> listUsers = _authRepo.ListUser();
                int itemsCount = listUsers.Count();
                listUsers = listUsers.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                listUsers.Sort(delegate (ApplicationUser user1, ApplicationUser user2) {
                    int compareDate = (user1.Roles.FirstOrDefault() ?? "").CompareTo(user2.Roles.FirstOrDefault() ?? "");
                    if (compareDate == 0) return user1.UserName.CompareTo(user2.UserName);
                    return compareDate;
                });
                
                Logs.debug($"ListItem {listUsers.Count} : \n {Newtonsoft.Json.JsonConvert.SerializeObject(listUsers, JsonSettings.SettingForNewtonsoft)}");

                return Task.FromResult(new JsonResult(new { message = "Success", data = listUsers, itemsCount = itemsCount }) { StatusCode = 200 });
            }catch (Exception ex) {
                Logs.debug("Exception: " + ex.StackTrace);
                return Task.FromResult(new JsonResult(new IdentityError() { Description = ex.Message, Code = "500" }) { StatusCode = 500 });
            }
        }

        // Create new user
        public async Task<JsonResult> OnPostInsertItem(ApplicationUser user)
        {
            try
            {
                Logs.debug($"InsertItem: {Newtonsoft.Json.JsonConvert.SerializeObject(user, JsonSettings.SettingForNewtonsoft)}");
                if (user == null) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };
                if (string.IsNullOrEmpty(user.UserName)) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };
                if (string.IsNullOrEmpty(user.Password)) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };

                // Check user exists
                ApplicationUser oldUser = await _authRepo.FindByUserName(user.UserName);
                if (oldUser != null) return new JsonResult(new { message = "Error", data = "Username is already used" }) { StatusCode = 200 };

                // Check role exists
                string roleId = user.Roles.FirstOrDefault();
                ApplicationRole roleUser = await _authRepo.GetRoleWithId(roleId);
                if (roleUser == null) return new JsonResult(new { message = "Error", data = "Role not found" }) { StatusCode = 200 };

                // Create user
                ApplicationUser userToCreate = new ApplicationUser() {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Provider = TypeProvider.Website,
                    Password = new Encrypt(user.Password).EncryptString()
                };
                IdentityResult result = await _authRepo.CreateUserWithPassword(userToCreate, user.Password);
                Logs.debug("Result: "+result);
                if (!result.Succeeded) return new JsonResult(new { message = "Error", data = result.Errors.ToString() }) { StatusCode = 200 };

                // Add role for user
                result = await _authRepo.AddToRoleId(roleId, userToCreate.UserName);
                Logs.debug("Result: " + result);
                if (!result.Succeeded) return new JsonResult(new { message = "Error", data = result.Errors.ToString() }) { StatusCode = 200 };

                return new JsonResult(new { message = "Success", data = userToCreate });
            }catch (Exception ex) {
                Logs.debug("Exception: " + ex.StackTrace);
                return new JsonResult(new IdentityError() { Description = ex.Message, Code = "500" }) { StatusCode = 500 };
            }
        }

        // Update user & password & roles
        public async Task<JsonResult> OnPostUpdateItem(ApplicationUser user)
        {
            try
            {
                Logs.debug($"UpdateItem: {Newtonsoft.Json.JsonConvert.SerializeObject(user, JsonSettings.SettingForNewtonsoft)}");
                if (user == null) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };
                if (string.IsNullOrEmpty(user.UserName)) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };
                if (string.IsNullOrEmpty(user.Password)) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };

                // Check user exists
                if (user.UserName == "cnttitk36@gmail.com") return new JsonResult(new { message = "Error", data = "This user can not update" }) { StatusCode = 200 };
                ApplicationUser oldUser = await _authRepo.FindById(user.Id);
                if (oldUser == null) return new JsonResult(new { message = "Error", data = "Can not update. User not found" }) { StatusCode = 200 };

                // Update Roles
                string newRoleId = user.Roles.FirstOrDefault();
                string oldRoleId = oldUser.Roles.FirstOrDefault();
                if (oldRoleId != newRoleId) {
                    Logs.debug($"Old role: {oldRoleId} --> New role: {newRoleId}");
                    if (!string.IsNullOrEmpty(oldRoleId)) {
                        IdentityResult isExits = await _authRepo.CheckInRoleId(oldRoleId, oldUser.UserName);
                        if (isExits.Succeeded)
                        {
                            await _authRepo.RemoveToRoleId(oldRoleId, oldUser.UserName);
                        }
                    }
                    if (!string.IsNullOrEmpty(newRoleId))
                    {
                        IdentityResult isExits = await _authRepo.CheckInRoleId(newRoleId, oldUser.UserName);
                        if (!isExits.Succeeded)
                        {
                            await _authRepo.AddToRoleId(newRoleId, oldUser.UserName);
                        }
                    }
                }
                // Update Roles ./

                // Change Password if OldPass != NewPass
                string oldPass = new Encrypt(oldUser.Password).DecryptString();
                string newPass = user.Password;
                if (!oldPass.Equals(newPass))
                {
                    Logs.debug($"Change pass oldPass: \"{oldPass}\", newPass: \"{newPass}\"");
                    await _authRepo.ChangePassword(oldUser.UserName, oldPass, newPass);
                    oldUser.Password = new Encrypt(newPass).EncryptString();
                }
                else
                {
                    oldUser.Password = oldUser.Password;
                }
                // Change Password if OldPass != NewPass ./

                oldUser.UserName = user.UserName;
                oldUser.NormalizedUserName = user.NormalizedUserName;
                oldUser.Email = user.Email;
                oldUser.NormalizedEmail = user.NormalizedEmail;
                oldUser.EmailConfirmed = user.EmailConfirmed;
                oldUser.PhoneNumber = user.PhoneNumber;
                oldUser.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                oldUser.TwoFactorEnabled = user.TwoFactorEnabled;
                oldUser.LockoutEnd = user.LockoutEnd;
                oldUser.LockoutEnabled = user.LockoutEnabled;
                oldUser.AccessFailedCount = user.AccessFailedCount;

                IdentityResult result = await _userManager.UpdateAsync(oldUser);
                Logs.debug("Result: "+result);
                if (!result.Succeeded) return new JsonResult(new { message = "Error", data = result.Errors.ToString() }) { StatusCode = 200 };

                return new JsonResult(new { message = "Success", data = user });
            }catch (Exception ex) {
                Logs.debug("Exception: " + ex.StackTrace);
                return new JsonResult(new IdentityError() { Description = ex.Message, Code = "500" }) { StatusCode = 500 };
            }
        }

        // Delete user
        public async Task<JsonResult> OnPostDeleteItem(ApplicationUser user)
        {
            try
            {
                Logs.debug("DeleteItem: " + Newtonsoft.Json.JsonConvert.SerializeObject(user, JsonSettings.SettingForNewtonsoft));
                if (user == null) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };
                if (string.IsNullOrEmpty(user.UserName)) return new JsonResult(new { message = "Error", data = "Please pass all the fields" }) { StatusCode = 200 };

                // Check user exists
                if (user.UserName == "cnttitk36@gmail.com") return new JsonResult(new { message = "Error", data = "This user can not delete" }) { StatusCode = 200 };
                ApplicationUser oldUser = await _authRepo.FindById(user.Id);
                if (oldUser == null) return new JsonResult(new { message = "Error", data = "Can not delete. User not found" }) { StatusCode = 200 };

                // Delete all role
                await _authRepo.RemoveToRoleId(oldUser.Roles.FirstOrDefault(), oldUser.UserName);
                // Delete all device
                _deviceRepo.RemoveAllDevice(oldUser.Id);
                // Delete user
                await _authRepo.DeleteUser(oldUser.UserName);
                
                return new JsonResult(new { message = "Success", data = user });
            }catch (Exception ex) {
                Logs.debug("Exception: " + ex.StackTrace);
                return new JsonResult(new IdentityError() { Description = ex.Message, Code = "500" }) { StatusCode = 500 };
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Core.Constants;
using App.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sophie.Controllers.API;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;
using Sophie.Resource.Entities;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class CreateShopModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly IShopRepository _shopRepository;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IOrderRepository _orderRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public Shop? Shop { get; set; }

        [BindProperty]
        public List<SelectListItem> ListPharmacist { get; set; }

        [BindProperty]
        public List<Shop> ListShops { get; set; }

        public string rootPathUpload = "";

        public CreateShopModel(IConfiguration config, IMapper mapper, IShopRepository shopRepository, UserManager<ApplicationUser> userManager,
            IPharmacistRepository pharmacistRepository, IOrderRepository orderRepository)
        {
            _config = config;
            _mapper = mapper;
            _shopRepository = shopRepository;
            _userManager = userManager;
            _pharmacistRepository = pharmacistRepository;
            _orderRepository = orderRepository;
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
        }

        public void OnGet(string shopId)
        {
            var listPharmacist = _pharmacistRepository.ListPharmacist(0, int.MaxValue);
            ListPharmacist = listPharmacist.Select(p => new SelectListItem { Value = p.PharmacistId, Text = p.Email }).ToList();
            if (!string.IsNullOrEmpty(shopId))
            {
                Shop = _shopRepository.FindByIdShop(shopId);
            }
            if (Shop == null)
            {
                Shop = new Shop()
                {
                    Type = TypeShop.Actived,
                    TransportPrice = 30000
                };
            }
        }

        public async Task<IActionResult> OnPostCreate(Shop model, IFormFile[] attachment)
        {
            foreach (var file in attachment)
            {
                if (file.Length / 1024 / 1024 >= 32)
                {
                    StatusMessage = "File more than 32MB";
                    return Page();
                }
            }
            if (string.IsNullOrEmpty(model.ShopName))
            {
                StatusMessage = "Error invalid input data";
                return Page();
            }

            if (string.IsNullOrEmpty(model.ShopId))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shop = _shopRepository.FindByIdPharmacist(userId);
                if (shop != null)
                {
                    StatusMessage = "Shop is exits";
                    return Page();
                }
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/shop";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.ShopImage) && model.ShopImage.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.ShopImage}")) System.IO.File.Delete($"{rootPathUpload}/{model.ShopImage}");
                    }

                    // upload new file
                    List<string> images = new List<string>();
                    foreach (var file in attachment)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var path = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        images.Add($"{uploadDirecotroy}/{fileName}");
                    }
                    model.ShopImage = images[0];
                }
                else // copy from image default
                {
                    string uploadDirecotroy = "/uploads/shop";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}.png";
                    System.IO.File.Copy($"{rootPathUpload}/cart/shop.png", $"{uploadPath}/{fileName}");
                    model.ShopImage = $"{uploadDirecotroy}/{fileName}";
                }
                _shopRepository.CreateShop(model);
                StatusMessage = "Shop created successfully";
            }
            else
            {
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/shop";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.ShopImage) && model.ShopImage.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.ShopImage}")) System.IO.File.Delete($"{rootPathUpload}/{model.ShopImage}");
                    }

                    // upload new file
                    List<string> images = new List<string>();
                    foreach (var file in attachment)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var path = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        images.Add($"{uploadDirecotroy}/{fileName}");
                    }
                    model.ShopImage = images[0];
                }
                _shopRepository.UpdateShop(model);
                StatusMessage = "Shop edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/Shop/ListShop");
        }

        public IActionResult OnGetDelete(string shopId)
        {
            Shop? item = _shopRepository.FindByIdShop(shopId);
            if (item == null)
            {
                StatusMessage = "Error: Shop not found";
                return Page();
            }
            var order = _orderRepository.FindExitsByShopId(item.ShopId);
            if (order != null)
            {
                StatusMessage = "Error: Shop is had order can't delete";
                return Page();
            }
            // remove old file if upload
            if (!string.IsNullOrEmpty(item.ShopImage) && item.ShopImage.IndexOf("uploads") >= 0)
            {
                if (System.IO.File.Exists($"{rootPathUpload}/{item.ShopImage}")) System.IO.File.Delete($"{rootPathUpload}/{item.ShopImage}");
            }
            _shopRepository.DeleteShop(item.ShopId);

            StatusMessage = "Shop deleted successfully";
            return LocalRedirect($"~/Admin/ShopPage/Shop/ListShop");
        }

        public IActionResult OnPostUpload(IFormFile[] file)
        {
            string uploadDirecotroy = "/uploads/shop";
            string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // upload new file
            List<string> images = new List<string>();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file[0].FileName)}";
            var path = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file[0].CopyTo(stream);
            }
            images.Add($"{uploadDirecotroy}/{fileName}");
            Shop.ShopImage = images[0];
            return Page();
        }

        public IActionResult OnPostGetTime(string name)
        {
            return new JsonResult(Shop);
        }

        public IActionResult OnPostFakeData()
        {
            var EmailAdmin = _config["EmailAdmin"];
            var newPharmacist = _pharmacistRepository.FindByEmailPharmacist(EmailAdmin);
            if (newPharmacist == null)
            {
                newPharmacist = new Pharmacist()
                {
                    PharmacistId = "",
                    TypeLogin = TypeLogin.Email,
                    Confirm = true,
                    Active = TypeActive.Actived,
                    PhoneNumber = "+84336371979",
                    Email = EmailAdmin ?? "pharmacies@gmail.com",
                    Username = "",
                    Password = "uEPO8M591zgc3bXSRDVijA==",
                    Firstname = "Trần Việt",
                    Lastname = "Thức",
                    NamePharmacist = "Dược sĩ Admin",
                    Birthdate = DateTimes.ParseExact("1989-06-26T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                    Address = "104 Hai Bà Trưng, Lê Hồng Phong, Quy Nhơn, Bình Định",
                    HomePhone = "",
                    Avatar = "",
                    Race = "",
                    Gender = "Male",
                    Language = "VI",
                    TwoFactorEnabled = false,
                    IsOnline = false,
                    VideoCallId = "",
                    VideoCallToken = "",
                    Notes = "",
                    DynamicField = "",

                };
                newPharmacist = _pharmacistRepository.CreatePharmacist(newPharmacist);
            }

            var shop = new Shop()
            {
                ShopId = Guid.NewGuid().ToString(),
                PharmacistId = newPharmacist.PharmacistId,
                ShopImage = "/img/New5.png",
                ShopAddress = "Địa chỉ shop hồ chí minh",
                ShopName = "Shop siêu thuốc",
                Type = TypeShop.Actived,
                Description = "Đây là shop bán siêu thuốc",
                TransportPrice = 30000
            };
            _shopRepository.CreateShop(shop);

            StatusMessage = "Shop fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Shop/CreateShop");
        }
    }
}

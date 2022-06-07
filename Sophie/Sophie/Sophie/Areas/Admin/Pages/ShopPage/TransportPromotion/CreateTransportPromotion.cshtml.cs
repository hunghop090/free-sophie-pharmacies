using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Core.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Sophie.Controllers.API;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class CreateTransportPromotionModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly ITransportPromotionRepository _transportPromotionRepository;
        private readonly IShopRepository _shopRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public TransportPromotion? TransportPromotion { get; set; }

        [BindProperty]
        public List<TransportPromotion> ListTransportPromotions { get; set; }

        public string rootPathUpload = "";

        [BindProperty(SupportsGet = true)]
        public List<Shop> ListShop { get; set; }

        [BindProperty]
        public List<SelectListItem> ListTypePay { get; set; } = Enum.GetValues(typeof(TypePay)).Cast<TypePay>().Select(v => new SelectListItem
        {
            Text = v.ToString(),
            Value = ((int)v).ToString()
        }).ToList();

        [BindProperty]
        public List<SelectListItem> ListTypeDiscount { get; set; } = Enum.GetValues(typeof(TypeTransportPromotionsDiscount)).Cast<TypeTransportPromotionsDiscount>().Select(v => new SelectListItem
        {
            Text = v.ToString(),
            Value = ((int)v).ToString()
        }).ToList();


        public CreateTransportPromotionModel(IConfiguration config, IMapper mapper, ITransportPromotionRepository transportPromotionRepository, IShopRepository shopRepository)
        {
            _config = config;
            _mapper = mapper;
            _transportPromotionRepository = transportPromotionRepository;
            _shopRepository = shopRepository;
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
        }

        public void OnGet(string transportPromotionId)
        {

            if (!string.IsNullOrEmpty(transportPromotionId))
            {
                TransportPromotion = _transportPromotionRepository.FindByIdTransportPromotion(transportPromotionId);
            }
            if (TransportPromotion == null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                TransportPromotion = new TransportPromotion()
                {
                    PharmacistId = userId,
                    Type = TypeEnum.Actived,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    TransportPromotionCode = RandomeCode()
                };
            }
            ListShop = _shopRepository.FindAll();
            if (ListShop == null)
            {
                StatusMessage = "Error: Shop not found";
            }
        }

        private string RandomeCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public IActionResult OnPostCreate(TransportPromotion model)
        {
            var shop = _shopRepository.FindByIdShop(model.ShopId);
            if (string.IsNullOrEmpty(model.TransportPromotionCode))
            {
                StatusMessage = "TransportPromotionCode is empty";
            }
            else
            if (string.IsNullOrEmpty(model.TransportPromotionId))
            {
                var code = _transportPromotionRepository.FindByCode(model.TransportPromotionCode);
                if (code != null && code.TransportPromotionId != model.TransportPromotionId)
                {
                    StatusMessage = "TransportPromotionCode is exists";
                }
            }
            if (shop == null)
            {
                StatusMessage = "Shop is not exists";
            }

            if (string.IsNullOrEmpty(model.TransportPromotionName))
            {
                StatusMessage = "Error invalid input data";
            }
            if (!string.IsNullOrEmpty(StatusMessage))
            {
                ListShop = _shopRepository.FindAll();
                if (ListShop == null)
                {
                    StatusMessage = "Error: Shop not found";
                }
                return Page();
            }

            if (string.IsNullOrEmpty(model.TransportPromotionId))
            {
                model.PharmacistId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _transportPromotionRepository.CreateTransportPromotion(model);
                StatusMessage = "TransportPromotion created successfully";
            }
            else
            {
                _transportPromotionRepository.UpdateTransportPromotion(model);
                StatusMessage = "TransportPromotion edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/TransportPromotion/ListTransportPromotion");
        }

        public IActionResult OnGetDelete(string transportPromotionId)
        {
            TransportPromotion? item = _transportPromotionRepository.FindByIdTransportPromotion(transportPromotionId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _shopRepository.FindByIdPharmacist(userId);
            if (item == null || item.ShopId != shop.ShopId)
            {
                StatusMessage = "Error: TransportPromotion not found";
            }
            else
            {
                _transportPromotionRepository.DeleteTransportPromotion(item.TransportPromotionId);

                StatusMessage = "TransportPromotion deleted successfully";
                return LocalRedirect($"~/Admin/ShopPage/TransportPromotion/ListTransportPromotion");
            }
            return Page();
        }

        public IActionResult OnPostFakeData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _shopRepository.FindAll().Where(x => x.Type == TypeShop.Actived).FirstOrDefault();
            if (shop == null)
            {
                StatusMessage = "shop is not exits";
                return LocalRedirect($"~/Admin/ShopPage/TransportPromotion/CreateTransportPromotion");
            }
            // transportPromotion 1
            var transportPromotion = new TransportPromotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                TransportPromotionName = "Miễn ship",
                TransportPromotionCode = RandomeCode(),
                TransportPromotionQuantity = 1000,
                TypePay = TypePay.Other,
                TypeDiscount = TypeTransportPromotionsDiscount.TypeTransportPromotionsDiscount_1,
                Price = 0,
                Discount = 100,
                MinBuget = 300000,
                Title = "Miễn ship: đơn hàng tối thiểu 300k",
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _transportPromotionRepository.CreateTransportPromotion(transportPromotion);
            // transportPromotion 2
            transportPromotion = new TransportPromotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                TransportPromotionName = "Giảm ship",
                TransportPromotionCode = RandomeCode(),
                TransportPromotionQuantity = 100,
                TypePay = TypePay.ZaloMomo,
                TypeDiscount = TypeTransportPromotionsDiscount.TypeTransportPromotionsDiscount_2,
                Price = 20000,
                Discount = 0,
                MinBuget = 120000,
                Title = "Giảm ship: giảm 20k, đơn hàng tối thiểu 120k",
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _transportPromotionRepository.CreateTransportPromotion(transportPromotion);

            StatusMessage = "TransportPromotion fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/TransportPromotion/CreateTransportPromotion");
        }

        public IActionResult OnPostDeleteFakeData()
        {
            Paging page = new Paging()
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
            };
            PagingResult<TransportPromotion> listTransportPromotion = _transportPromotionRepository.ListTransportPromotion(page);
            foreach (var item in listTransportPromotion.Result)
            {
                _transportPromotionRepository.DeleteTransportPromotion(item.TransportPromotionId);
            }

            StatusMessage = "TransportPromotion delete fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/TransportPromotion/CreateTransportPromotion");
        }
    }
}

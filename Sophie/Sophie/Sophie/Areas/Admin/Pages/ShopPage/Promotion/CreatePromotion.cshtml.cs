using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Core.Constants;
using AutoMapper;
using awsTestUpload;
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
    public class CreatePromotionModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly IPromotionRepository _promotionRepository;
        private readonly IShopRepository _shopRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Promotion? Promotion { get; set; }


        [BindProperty]
        public Shop? Shop { get; set; }

        [BindProperty]
        public List<Promotion> ListPromotions { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Shop> ListShop { get; set; }


        [BindProperty]
        public List<SelectListItem> ListTypePay { get; set; } = Enum.GetValues(typeof(TypePay)).Cast<TypePay>().Select(v => new SelectListItem
        {
            Text = v.ToString(),
            Value = ((int)v).ToString()
        }).ToList();

        [BindProperty]
        public List<SelectListItem> ListTypeDiscount { get; set; } = Enum.GetValues(typeof(TypePromotionsDiscount)).Cast<TypePromotionsDiscount>().Select(v => new SelectListItem
        {
            Text = v.ToString(),
            Value = ((int)v).ToString()
        }).ToList();

        [BindProperty]
        public List<ImageInfo> ListImageInfo { get; set; } = new List<ImageInfo>();


        public string rootPathUpload = "";

        public CreatePromotionModel(IConfiguration config, IMapper mapper, IPromotionRepository promotionRepository, IShopRepository shopRepository)
        {
            _config = config;
            _mapper = mapper;
            _promotionRepository = promotionRepository;
            _shopRepository = shopRepository;
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
        }

        public void OnGet(string promotionId)
        {

            if (!string.IsNullOrEmpty(promotionId))
            {
                Promotion = _promotionRepository.FindByIdPromotion(promotionId);
            }
            if (Promotion == null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                Promotion = new Promotion()
                {
                    PharmacistId = userId,
                    Type = TypeEnum.Actived,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    PromotionCode = RandomeCode()
                };
            }
            if (Promotion.Banner != null)
            {
                foreach (var image in Promotion.Banner)
                {
                    AmazonUploader AU = new AmazonUploader(_config);
                    ListImageInfo.Add(new ImageInfo
                    {
                        KeyImage = image,
                        Url = AU.GetUrlImage(image, "/promotions/" + Promotion.PromotionId)
                    });
                }
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


        public IActionResult OnPostCreate(Promotion model, IFormFile[] attachment)
        {
            if (string.IsNullOrEmpty(model.PromotionCode))
            {
                StatusMessage = "PromotionCode is empty";
            }
            else
         if (string.IsNullOrEmpty(model.PromotionId))
            {
                var code = _promotionRepository.FindByCode(model.PromotionCode);
                if (code != null)
                {
                    StatusMessage = "TransportPromotionCode is exists";
                    return Page();
                }
            }
            var shop = _shopRepository.FindByIdShop(model.ShopId);
            foreach (var file in attachment)
            {
                if (file.Length / 1024 / 1024 >= 32)
                {
                    StatusMessage = "File more than 32MB";
                    return Page();
                }
            }
            if (model.Banner == null && attachment.Length == 0)
            {
                StatusMessage = "Error: Banner is empty";
            }
            if (shop == null)
            {
                StatusMessage = "Shop is not exists";
            }

            if (string.IsNullOrEmpty(model.PromotionName))
            {
                StatusMessage = "Error PromotionName is empty";
            }

            if (string.IsNullOrEmpty(model.Content))
            {
                StatusMessage = "Error Content is empty";
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
            AmazonUploader AU = new AmazonUploader(_config);

            if (string.IsNullOrEmpty(model.PromotionId))
            {
                if (attachment.Length > 0)
                {
                    foreach (var item in attachment)
                    {
                        var keyFile = AU.SendMyFileToS3(item, item.FileName, "promotions/" + model.PromotionId).Result;
                        if (!string.IsNullOrEmpty(keyFile))
                            model.Banner.Add(keyFile);
                    }
                }
                model.PharmacistId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _promotionRepository.CreatePromotion(model);
                StatusMessage = "Promotion created successfully";
            }
            else
            {
                var oldPromotion = _promotionRepository.FindByIdPromotion(model.PromotionId);
                if (oldPromotion == null)
                {
                    StatusMessage = "Promotion không tồn tại";
                    return Page();
                };
                List<string> deleteImages = new List<string>();
                foreach (var image in oldPromotion.Banner)
                {
                    if (model.Banner != null && !model.Banner.Contains(image) && !string.IsNullOrEmpty(image))
                        deleteImages.Add(image);
                }
                if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);
                if (attachment.Length > 0)
                {
                    foreach (var item in attachment)
                    {
                        var keyFile = AU.SendMyFileToS3(item, item.FileName, "products/" + model.PromotionId).Result;
                        if (!string.IsNullOrEmpty(keyFile))
                            model.Banner.Add(keyFile);
                    }
                }
                _promotionRepository.UpdatePromotion(model);
                StatusMessage = "Promotion edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/Promotion/ListPromotion");
        }

        public IActionResult OnGetDelete(string promotionId)
        {
            AmazonUploader AU = new AmazonUploader(_config);
            Promotion? item = _promotionRepository.FindByIdPromotion(promotionId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _shopRepository.FindByIdPharmacist(userId);
            if (item == null || item.ShopId != shop.ShopId)
            {
                StatusMessage = "Error: Promotion not found";
            }
            else
            {
                _promotionRepository.DeletePromotion(item.PromotionId);
                List<string> deleteImages = new List<string>();
                foreach (var image in item.Banner)
                {
                    if (!string.IsNullOrEmpty(image))
                        deleteImages.Add(image);
                }
                if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);

                StatusMessage = "Promotion deleted successfully";
                return LocalRedirect($"~/Admin/ShopPage/Promotion/ListPromotion");
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
                return LocalRedirect($"~/Admin/ShopPage/Promotion/CreatePromotion");
            }
            // promotion 1
            var promotion = new Promotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                PromotionName = "Giảm giá khi thanh toán Zalo",
                PromotionCode = RandomeCode(),
                PromotionQuantity = 50000,
                TypePay = TypePay.Zalo,
                TypeDiscount = TypePromotionsDiscount.TypePromotionsDiscount_1,//- giảm (zalo, momo): giảm 25% tối đa 25k, đơn hàng tối thiểu 90k
                Price = 0,
                MinBuget = 0,
                Discount = 25,
                MaxPriceDiscount = 25000,
                Title = "Mua liền tay, tiết kiệm lớn, gửi quà cho bạn bè, người thân nhé",
                Content = @"<p><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;""><strong style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font-variant-numeric: inherit; font-variant-east-asian: inherit; font-stretch: inherit; font-size: 14pt; line-height: inherit; font-family: sans-serif; vertical-align: baseline; font-weight: bold !important;""><span style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font: inherit; vertical-align: baseline; color: rgb(0, 0, 0);"">Chương trình ưu đãi khi thanh toán qua Zalo</span></strong></span></p>
<ul><li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Giảm giá 25% tối đa 25k cho đơn hàng tối thiểu 90k khi thanh toán bằng Zalo</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Thời gian chương trình: 01/01/2022 - 30/12/2022</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Số lượng: 50000</span></li></ul>",
                Banner = new List<string>() { "sample/promotions/promotion_1.png", "sample/promotions/promotion.mp4" },
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _promotionRepository.CreatePromotion(promotion);

            // promotion 2
            promotion = new Promotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                PromotionName = "Giảm giá khi thanh toán Momo",
                PromotionCode = RandomeCode(),
                PromotionQuantity = 50000,
                TypePay = TypePay.Zalo,
                TypeDiscount = TypePromotionsDiscount.TypePromotionsDiscount_2,//- giảm (zalo, momo): giảm 25k, đơn hàng tối thiểu 120k.
                Price = 25000,
                MinBuget = 120000,
                Discount = 0,
                MaxPriceDiscount = 0,
                Title = "Mua liền tay, tiết kiệm lớn, gửi quà cho bạn bè, người thân nhé",
                Content = @"<p><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;""><strong style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font-variant-numeric: inherit; font-variant-east-asian: inherit; font-stretch: inherit; font-size: 14pt; line-height: inherit; font-family: sans-serif; vertical-align: baseline; font-weight: bold !important;""><span style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font: inherit; vertical-align: baseline; color: rgb(0, 0, 0);"">Chương trình ưu đãi khi thanh toán qua Zalo</span></strong></span></p>
<ul><li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Giảm giá 25k cho đơn hàng tối thiểu 120k khi thanh toán bằng Momo</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Thời gian chương trình: 01/01/2022 - 30/12/2022</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Số lượng: 50000</span></li></ul>",
                Banner = new List<string>() { "sample/promotions/promotion_1.png", "sample/promotions/promotion.mp4" },
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _promotionRepository.CreatePromotion(promotion);

            // promotion 3
            promotion = new Promotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                PromotionName = "Giảm giá khi thanh toán qua Zalo hoặc Momo",
                PromotionCode = RandomeCode(),
                PromotionQuantity = 10000,
                TypePay = TypePay.ZaloMomo,
                TypeDiscount = TypePromotionsDiscount.TypePromotionsDiscount_1,//- giảm (zalo, momo): giảm 25% tối đa 25k, đơn hàng tối thiểu 90k
                Price = 0,
                MinBuget = 300000,
                Discount = 10,
                MaxPriceDiscount = 25000,
                Title = "Mua ngày hoàn tiền khi thanh toán với Zalo hoặc Momo",
                Content = @"<p><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;""><strong style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font-variant-numeric: inherit; font-variant-east-asian: inherit; font-stretch: inherit; font-size: 14pt; line-height: inherit; font-family: sans-serif; vertical-align: baseline; font-weight: bold !important;""><span style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font: inherit; vertical-align: baseline; color: rgb(0, 0, 0);"">Chương trình ưu đãi khi thanh toán qua Zalo</span></strong></span></p>
<ul><li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Giảm giá 10% tối đa 25k cho đơn hàng tối thiểu 300k khi thanh toán bằng Zalo hoặc Momo</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Thời gian chương trình: 01/01/2022 - 30/12/2022</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Số lượng: 10000</span></li></ul>",
                Banner = new List<string>() { "sample/promotions/promotion_1.png", "sample/promotions/promotion.mp4" },
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _promotionRepository.CreatePromotion(promotion);

            // promotion 4
            promotion = new Promotion()
            {
                ShopId = shop.ShopId,
                PharmacistId = userId,
                PromotionName = "Giảm giá tất cả hình thức thanh toán",
                PromotionCode = RandomeCode(),
                PromotionQuantity = 5000,
                TypePay = TypePay.Other,
                TypeDiscount = TypePromotionsDiscount.TypePromotionsDiscount_2,//- giảm (zalo, momo): giảm 25k, đơn hàng tối thiểu 120k.
                Price = 30000,
                MinBuget = 300000,
                Discount = 0,
                MaxPriceDiscount = 0,
                Title = "Mua ngày hoàn tiền khi thanh toán với bất kỳ hình thức thanh toán",
                Content = @"<p><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;""><strong style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font-variant-numeric: inherit; font-variant-east-asian: inherit; font-stretch: inherit; font-size: 14pt; line-height: inherit; font-family: sans-serif; vertical-align: baseline; font-weight: bold !important;""><span style=""transition: all 0.5s ease 0s; margin: 0px; padding: 0px; border: 0px; font: inherit; vertical-align: baseline; color: rgb(0, 0, 0);"">Chương trình ưu đãi khi thanh toán qua Zalo</span></strong></span></p>
<ul><li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Giảm giá 30k cho đơn hàng tối thiểu 300k</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Thời gian chương trình: 01/01/2022 - 30/12/2022</span></li>
<li><span style=""color:= rgb(100, 100, 100); font-family: Roboto, Arial, Helvetica, sans-serif; font-size: 18px;"">Số lượng: 5000</span></li></ul>",
                Banner = new List<string>() { "sample/promotions/promotion_1.png", "sample/promotions/promotion.mp4" },
                StartDate = DateTimes.ParseExact("2022-01-01T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                EndDate = DateTimes.ParseExact("2022-12-30T00:00:00", DateTimes.format16()),//yyyy-MM-ddTHH:mm:ss
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _promotionRepository.CreatePromotion(promotion);

            StatusMessage = "Promotion fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Promotion/CreatePromotion");
        }

        public IActionResult OnPostDeleteFakeData()
        {
            AmazonUploader AU = new AmazonUploader(_config);
            Paging page = new Paging()
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
            };
            PagingResult<Promotion> listPromotion = _promotionRepository.ListPromotion(page);
            List<string> deleteImages = new List<string>();
            foreach (var item in listPromotion.Result)
            {
                _promotionRepository.DeletePromotion(item.PromotionId);
                foreach (var image in item.Banner)
                {
                    if (!string.IsNullOrEmpty(image) && !deleteImages.Contains(image))
                        deleteImages.Add(image);
                }
            }
            if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);

            StatusMessage = "Promotion delete fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Promotion/CreatePromotion");
        }
    }
}

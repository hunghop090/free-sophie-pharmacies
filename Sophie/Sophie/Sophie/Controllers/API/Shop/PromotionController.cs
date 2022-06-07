using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using log4net;
using App.Core.Constants;
using App.Core.Policy;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using System.Collections.Generic;
using Sophie.Resource.Model;
using Microsoft.Extensions.Logging;
using Sophie.Model;
using Sophie.Resource.Dtos.Shop;
using Sophie.Units;
using App.Core.Units;
using Microsoft.Extensions.Configuration;
using awsTestUpload;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API_ACCOUNT)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v5")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.Account, IsAnd = false)]
    public class PromotionController : BaseAPIController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(PromotionController));
        private readonly ILogger<PromotionController> _logger;

        private readonly IConfiguration _config;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IPharmacistRepository _harmacistRepository;

        public PromotionController(ILogger<PromotionController> logger, IConfiguration config, IPromotionRepository promotionRepository, IShopRepository shopRepository,
            IPharmacistRepository pharmacistRepository)
        {
            _logger = logger;
            _config = config;
            _promotionRepository = promotionRepository;
            _shopRepository = shopRepository;
            _harmacistRepository = pharmacistRepository;
        }

        /// <summary>
        /// Get list promotion (Lấy danh sách chương trình khuyến mãi giảm giá theo cửa hàng)
        /// </summary>
        /// <param name="shopIds"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List promotion</returns>
        // Post: api/shop/Promotion/ListPromotionByShopIds
        [HttpPost("ListPromotionByShopIds")]
        public IActionResult ListPromotionByShopIds(string? shopIds = null, int skip = 0, int limit = 99)
        {
            try
            {
                if (string.IsNullOrEmpty(shopIds))
                {
                    var email = _configuration["EmailAdmin"];
                    var Pharmacist = _harmacistRepository.FindByEmailPharmacist(email);
                    var shop = _shopRepository.FindByIdPharmacist(Pharmacist?.PharmacistId);
                    if (shop != null)
                    {
                        shopIds = shop.ShopId;
                    }
                }
                FilterWithId filter = new FilterWithId()
                {
                    Id = shopIds,
                    PageIndex = skip / (limit == 0 ? 1 : limit),
                    PageSize = limit
                };
                PagingResult<Promotion> listPromotion = _promotionRepository.ListPromotionByShopIds(filter);
                List<PromotionInListDto> listPromotionDto = _mapper.Map<List<PromotionInListDto>>(listPromotion.Result);
                AmazonUploader AU = new AmazonUploader(_config);
                foreach (var promotion in listPromotionDto)
                {
                    if (promotion.Banner != null)
                    {
                        var _listImage = new List<string>();
                        foreach (var image in promotion.Banner)
                        {
                            _listImage.Add(AU.GetUrlImage(image, "/promotions/" + promotion.PromotionId));
                        };
                        promotion.Banner = _listImage;
                    }
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listPromotionDto, Total = listPromotion.Total });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Promotion/ListPromotion", $"Error user get list promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Promotion/ListPromotion", ex);
            }
        }

        /// <summary>
        /// Get list promotion (Tìm kiếm danh sách chương trình khuyến mãi giảm giá active theo PromotionName)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List promotion</returns>
        // GET: api/shop/Promotion/SearchPromotion
        [HttpGet("SearchPromotion")]
        public IActionResult SearchPromotion(string? search = "", int skip = 0, int limit = 99)
        {
            try
            {
                Paging paging = new Paging()
                {
                    search = search,
                    PageIndex = skip / (limit == 0 ? 1 : limit),
                    PageSize = limit
                };
                PagingResult<Promotion> listPromotion = _promotionRepository.ListPromotionActive(paging);
                List<PromotionInListDto> listPromotionDto = _mapper.Map<List<PromotionInListDto>>(listPromotion.Result);
                AmazonUploader AU = new AmazonUploader(_config);
                foreach (var promotion in listPromotionDto)
                {
                    if (promotion.Banner != null)
                    {
                        var _listImage = new List<string>();
                        foreach (var image in promotion.Banner)
                        {
                            _listImage.Add(AU.GetUrlImage(image, "/promotions/" + promotion.PromotionId));
                        };
                        promotion.Banner = _listImage;
                    }
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listPromotionDto, Total = listPromotion.Total });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Promotion/SearchPromotion", $"Error user get search list promotion active", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Promotion/SearchPromotion", ex);
            }
        }

        /// <summary>
        /// Get promotion by code (Kiểm tra mã khuyến mãi giảm giá)
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns>Promotion</returns>
        // GET: api/shop/Promotion/FindByCode
        [HttpGet("FindByCode")]
        public IActionResult FindByCode(string promotionCode)
        {
            try
            {
                Promotion promotion = _promotionRepository.FindByCode(promotionCode);
                if (promotion == null) return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_NOTFOUND"), this.ControllerContext));

                if (promotion.Type != TypeEnum.Actived || !(promotion.StartDate < DateTime.Now && promotion.EndDate > DateTime.Now))
                    return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_EXPIRED"), this.ControllerContext));

                PromotionDto promotionDto = _mapper.Map<PromotionDto>(promotion);
                AmazonUploader AU = new AmazonUploader(_config);
                var _listImage = new List<string>();
                foreach (var image in promotionDto.Banner)
                {
                    _listImage.Add(AU.GetUrlImage(image, "/promotions/" + promotion.PromotionId));
                };
                promotionDto.Banner = _listImage;

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = promotionDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Promotion/FindByCode", $"Error user find promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Promotion/FindByCode", ex);
            }
        }

        /// <summary>
        /// Get promotion by id (Lấy thông tin chi tiết chương trình khuyến mãi giảm giá)
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns>Promotion</returns>
        // GET: api/shop/Promotion/GetByPromotionId
        [HttpGet("GetByPromotionId")]
        public IActionResult GetByPromotionId(string promotionId)
        {
            try
            {
                Promotion promotion = _promotionRepository.FindByIdPromotion(promotionId);
                if (promotion == null) return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_NOTFOUND"), this.ControllerContext));

                PromotionDto promotionDto = _mapper.Map<PromotionDto>(promotion);
                AmazonUploader AU = new AmazonUploader(_config);
                var _listImage = new List<string>();
                foreach (var image in promotionDto.Banner)
                {
                    _listImage.Add(AU.GetUrlImage(image, "/promotions/" + promotion.PromotionId));
                };
                promotionDto.Banner = _listImage;

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = promotionDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Promotion/GetByPromotionId", $"Error user get detail promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Promotion/GetByPromotionId", ex);
            }
        }
    }
}

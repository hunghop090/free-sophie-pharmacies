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
using Sophie.Resource.Dtos.Shop;
using Sophie.Model;
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
    public class TransportPromotionController : BaseAPIController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(TransportPromotionController));
        private readonly ILogger<TransportPromotionController> _logger;

        private readonly IConfiguration _config;
        private readonly ITransportPromotionRepository _transportPromotionRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IPharmacistRepository _harmacistRepository;

        public TransportPromotionController(ILogger<TransportPromotionController> logger, IConfiguration config, ITransportPromotionRepository transportPromotionRepository,
            IPharmacistRepository harmacistRepository, IShopRepository shopRepository)
        {
            _logger = logger;
            _config = config;
            _transportPromotionRepository = transportPromotionRepository;
            _shopRepository = shopRepository;
            _harmacistRepository = harmacistRepository;
        }

        /// <summary>
        /// Get list transportPromotion (Lấy danh sách chương trình khuyến mãi vận chuyển theo cửa hàng)
        /// </summary>
        /// <param name="shopIds"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List transportPromotion</returns>
        // Post: api/shop/TransportPromotion/ListTransportPromotionByShopIds
        [HttpPost("ListTransportPromotionByShopIds")]
        public IActionResult ListTransportPromotionByShopIds(string? shopIds = null, int skip = 0, int limit = 99)
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
                PagingResult<TransportPromotion> listTransportPromotion = _transportPromotionRepository.ListTransportPromotionByShopIds(filter);
                List<TransportPromotionInListDto> listTransportPromotionDto = _mapper.Map<List<TransportPromotionInListDto>>(listTransportPromotion.Result);

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listTransportPromotionDto, Total = listTransportPromotion.Total });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/TransportPromotion/ListTransportPromotionByShopIds", $"Error user get list transport promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/TransportPromotion/ListTransportPromotionByShopIds", ex);
            }
        }

        /// <summary>
        /// Get list promotion (Tìm kiếm danh sách chương trình khuyến mãi vận chuyển active theo TransportPromotionName)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List promotion</returns>
        // GET: api/shop/Promotion/SearchTransportPromotion
        [HttpGet("SearchTransportPromotion")]
        public IActionResult SearchTransportPromotion(string? search = "", int skip = 0, int limit = 99)
        {
            try
            {
                Paging paging = new Paging()
                {
                    search = search,
                    PageIndex = skip / (limit == 0 ? 1 : limit),
                    PageSize = limit
                };
                PagingResult<TransportPromotion> listTransportPromotion = _transportPromotionRepository.ListPromotionActive(paging);
                List<TransportPromotionInListDto> listTransportPromotionDto = _mapper.Map<List<TransportPromotionInListDto>>(listTransportPromotion.Result);

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listTransportPromotionDto, Total = listTransportPromotion.Total });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/TransportPromotion/SearchTransportPromotion", $"Error user get search list transport promotion active", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/TransportPromotion/SearchTransportPromotion", ex);
            }
        }

        /// <summary>
        /// Get transportPromotion by code (Kiểm tra mã khuyến mãi vận chuyển)
        /// </summary>
        /// <param name="transportPromotionCode"></param>
        /// <returns>TransportPromotion</returns>
        // GET: api/shop/TransportPromotion/FindByCode
        [HttpGet("FindByCode")]
        public IActionResult FindByCode(string transportPromotionCode)
        {
            try
            {
                TransportPromotion transportPromotion = _transportPromotionRepository.FindByCode(transportPromotionCode);
                if (transportPromotion == null) return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_TRANSPORT_NOTFOUND"), this.ControllerContext));

                if (transportPromotion.Type != TypeEnum.Actived || !(transportPromotion.StartDate < DateTime.Now && transportPromotion.EndDate > DateTime.Now))
                    return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_TRANSPORT_EXPIRED"), this.ControllerContext));

                TransportPromotionDto transportPromotionDto = _mapper.Map<TransportPromotionDto>(transportPromotion);

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = transportPromotionDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/TransportPromotion/FindByCode", $"Error user find transport promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/TransportPromotion/FindByCode", ex);
            }
        }

        /// <summary>
        /// Get transportPromotion by id (Lấy thông tin chi tiết chương trình khuyến mãi vận chuyển)
        /// </summary>
        /// <param name="transportPromotionId"></param>
        /// <returns>TransportPromotion</returns>
        // GET: api/shop/TransportPromotion/GetByTransportPromotionId
        [HttpGet("GetByTransportPromotionId")]
        public IActionResult GetByTransportPromotionId(string transportPromotionId)
        {
            try
            {
                TransportPromotion transportPromotion = _transportPromotionRepository.FindByIdTransportPromotion(transportPromotionId);
                if (transportPromotion == null) return ResponseBadRequest(new CustomBadRequest(localizer("PROMOTION_TRANSPORT_NOTFOUND"), this.ControllerContext));
                TransportPromotionDto transportPromotionDto = _mapper.Map<TransportPromotionDto>(transportPromotion);

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = transportPromotionDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/TransportPromotion/GetByTransportPromotionId", $"Error user get detail transport promotion", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/TransportPromotion/GetByTransportPromotionId", ex);
            }
        }
    }
}

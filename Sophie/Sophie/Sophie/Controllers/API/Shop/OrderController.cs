using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using log4net;
using App.Core.Constants;
using App.Core.Policy;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using awsTestUpload;
using Microsoft.Extensions.Configuration;
using Sophie.Resource.Dtos.Shop;
using AutoMapper;
using App.Core.Units;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sophie.Model;
using System.Threading.Tasks;
using Sophie.Units;
using Sophie.Resource.Entities;
using System.ComponentModel;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API_ACCOUNT)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v5")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.Account, IsAnd = false)]
    public class OrderController : BaseAPIController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(OrderController));
        private readonly ILogger<OrderController> _logger;

        private readonly IOrderRepository _orderRepository;
        private readonly IDraftOrderRepository _draftOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _config;
        private readonly IAccountRepository _accountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ITransportPromotionRepository _tPromotionRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IShopRepository _shopRepository;

        public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository, IConfiguration config, IDraftOrderRepository draftOrderRepository, IProductRepository productRepository,
            IAccountRepository accountRepository, IPromotionRepository promotionRepository, ITransportPromotionRepository transportPromotionRepository,
            IAddressRepository addressRepository, IShopRepository shopRepository)
        {
            _logger = logger;
            _draftOrderRepository = draftOrderRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _config = config;
            _accountRepository = accountRepository;
            _promotionRepository = promotionRepository;
            _tPromotionRepository = transportPromotionRepository;
            _addressRepository = addressRepository;
            _shopRepository = shopRepository;
        }

        public class ValidateResult
        {
            public string Message { get; set; }
            public bool Result { get; set; }
        }

        /// <summary>
        /// Get order by accountId (Lấy thông tin đơn hàng theo accountId)
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>Order</returns>
        // GET: api/shop/Order/GetOrderHistory
        [HttpGet("GetOrderHistory")]
        public IActionResult GetOrderHistory(string? accountId, int skip = 0, int limit = 99)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId)) accountId = GetUserIdAuth();
                Account account = _accountRepository.FindByIdAccount(accountId);
                if (account == null) return ResponseBadRequest(new CustomBadRequest(localizer("BASE_USER_AUTH_NOT_FOUND"), this.ControllerContext));

                FilterWithId filter = new FilterWithId()
                {
                    Id = accountId,
                    PageIndex = skip / (limit == 0 ? 1 : limit),
                    PageSize = limit,
                    sortName = "Created",
                    sort = "desc"
                };
                AmazonUploader AU = new AmazonUploader(_config);
                PagingResult<OrderGroupBy> listOrderGroup = _orderRepository.FindByAccountId(filter);
                if (listOrderGroup == null) return ResponseBadRequest(new CustomBadRequest(localizer("ORDER_NOTFOUND"), this.ControllerContext));
                if (listOrderGroup.Result.FirstOrDefault().AccountId != account.AccountId) return ResponseBadRequest(new CustomBadRequest(localizer("DRAFT_ORDER_NOT_IN_ACCOUNT"), this.ControllerContext));

                List<OrderGroupByDto> listOrderGroupDto = _mapper.Map<List<OrderGroupByDto>>(listOrderGroup.Result);
                foreach (OrderGroupByDto data in listOrderGroupDto)
                {
                    data.Price = data.ListOrder.Sum(x => x.Price);
                    data.TransportPromotionPrice = data.ListOrder.Sum(x => x.TransportPromotionPrice);
                    data.TransportPrice = data.ListOrder.Sum(x => x.TransportPrice);
                    data.PromotionPrice = data.ListOrder.Sum(x => x.PromotionPrice);
                    data.AccountName = data.ListOrder.FirstOrDefault().AccountName;
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listOrderGroupDto, Total = listOrderGroup.Total });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/GetById", $"Error user get list history order", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/GetById", ex);
            }
        }

        /// <summary>
        /// Get order by transactionId (Lấy thông tin đơn hàng theo transactionId)
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>Order</returns>
        // GET: api/shop/Order/GetByTransactionId
        [HttpGet("GetByTransactionId")]
        public IActionResult GetByTransactionId(string transactionId)
        {
            try
            {
                AmazonUploader AU = new AmazonUploader(_config);
                List<Order> listOrders = _orderRepository.FindByIdTransactionId(transactionId);
                if (listOrders == null) return ResponseBadRequest(new CustomBadRequest(localizer("ORDER_NOTFOUND"), this.ControllerContext));

                List<OrderDto> listOrderDto = _mapper.Map<List<OrderDto>>(listOrders);
                List<string> shopids = new List<String>();
                foreach (OrderDto orderDto in listOrderDto)
                {
                    shopids.Add(orderDto.ShopId);
                    foreach (ProductOrder product in orderDto.ListProduct)
                    {
                        if (product.ProductImages != null)
                        {
                            List<string> listImage = new List<string>();
                            foreach (string image in product.ProductImages)
                            {
                                listImage.Add(AU.GetUrlImage(image, "/products/" + product.ProductId + "/"));
                            }
                            product.ProductImages = listImage;
                        }
                    };
                }

                List<Shop> listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
                foreach (OrderDto data in listOrderDto)
                {
                    data.ShopName = listShop.Where(x => x.ShopId == data.ShopId).FirstOrDefault()?.ShopName;
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listOrderDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/GetByTransactionId", $"Error user get list order by transactionId", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/GetByTransactionId", ex);
            }
        }

        public class SaveOrderResult
        {
            public DraftOrderDto DraftOrder { get; set; }
            public List<OrderDto> ListOrder { get; set; }
        }

        /// <summary>
        /// Get order by accountId (Lấy thông tin giỏ hàng theo accountId, không truyền accountId mặc định lấy account đang đăng nhập)
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Order</returns>
        // GET: api/shop/Order/GetByAccountId
        [HttpGet("GetByAccountId")]
        public IActionResult GetByAccountId(string? accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId)) accountId = GetUserIdAuth();
                Account account = _accountRepository.FindByIdAccount(accountId);
                if (account == null) return ResponseBadRequest(new CustomBadRequest(localizer("BASE_USER_AUTH_NOT_FOUND"), this.ControllerContext));

                AmazonUploader AU = new AmazonUploader(_config);
                DraftOrder draftOrder = _draftOrderRepository.FindByAccountId(accountId);
                if (draftOrder == null) return ResponseBadRequest(new CustomBadRequest(localizer("ORDER_NOTFOUND"), this.ControllerContext));
                DraftOrderDto draftOrderDto = _mapper.Map<DraftOrderDto>(draftOrder);
                if (draftOrderDto.ListProduct != null)
                {
                    List<Product> listProduct = _productRepository.FindByProductIds(String.Join(",", draftOrderDto.ListProduct.Select(x => x.ProductId)));
                    foreach (ProductOrderDto product in draftOrderDto.ListProduct)
                    {
                        Product detailProduct = listProduct.FirstOrDefault(x => x.ProductId == product.ProductId);
                        List<string> listImage = new List<string>();
                        foreach (string image in product.ProductImages)
                        {
                            listImage.Add(AU.GetUrlImage(image, "/products/" + product.ProductId + "/"));
                        };
                        product.ProductImages = listImage;
                        product.ProductPrice = (long)detailProduct?.ProductPrice;
                        product.ProductNumber = detailProduct?.ProductNumber;
                        product.PurchasedNumber = detailProduct?.PurchasedNumber;
                        product.ProductRealPrice = detailProduct?.ProductRealPrice;
                        product.SellOver = detailProduct?.SellOver;
                    }
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = draftOrderDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/GetByAccountId", $"Error user get detail draft order by accountId", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/GetByAccountId", ex);
            }
        }

        /// <summary>
        /// Create update order (Tạo mới hoặc cập nhật thông tin giỏ hàng)
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Order</returns>
        // Post: api/shop/Order/CreateOrUpdateOrder
        [HttpPost("CreateOrUpdateOrder")]
        public IActionResult CreateOrUpdateOrder(DraftOrderCreateOrUpdateDto order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.AccountId)) order.AccountId = GetUserIdAuth();
                Account account = _accountRepository.FindByIdAccount(order.AccountId);
                if (account == null) return ResponseBadRequest(new CustomBadRequest(localizer("BASE_USER_AUTH_NOT_FOUND"), this.ControllerContext));

                DraftOrder draftOrder = _mapper.Map<DraftOrder>(order);

                // Validate
                DraftOrder draftOrderAccount = _draftOrderRepository.FindByAccountId(draftOrder.AccountId);
                if (string.IsNullOrEmpty(draftOrder.DraftOrderId) && draftOrderAccount != null && string.IsNullOrEmpty(draftOrderAccount.DraftOrderId))
                {
                    return ResponseBadRequest(new CustomBadRequest(localizer("DRAFT_ORDER_NOT_IN_ACCOUNT"), this.ControllerContext));
                }
                if (!string.IsNullOrEmpty(draftOrder.DraftOrderId) && (draftOrderAccount == null || draftOrderAccount.DraftOrderId != draftOrder.DraftOrderId))
                {
                    return ResponseBadRequest(new CustomBadRequest(localizer("DRAFT_ORDER_NOT_IN_ACCOUNT"), this.ControllerContext));
                }

                // Caculator price Promotion & TransportPromotion if exits ListProduct
                List<Product> listProduct = _productRepository.FindByProductIds(string.Join(",", draftOrder.ListProduct.Select(x => x.ProductId)));
                if (draftOrder.ListProduct != null && draftOrder.ListProduct.Count > 0)
                {
                    foreach (ProductOrder product in draftOrder.ListProduct)
                    {
                        Product detailProduct = listProduct.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                        if (detailProduct == null || product.Quantity <= 0) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_NOTFOUND_QUALITY_EMPTY"), this.ControllerContext));

                        // Not update data from mobile, only update data from BE
                        product.CategoryId = detailProduct.CategoryId;
                        product.SubCategoryId = detailProduct.SubCategoryId;
                        product.ShopId = detailProduct.ShopId;
                        product.PharmacistId = detailProduct.PharmacistId;
                        product.ProductName = detailProduct.ProductName;
                        product.ProductImages = detailProduct.ProductImages;
                        product.ProductPrice = (long)(detailProduct.ProductPrice ?? 0);
                    }

                    // Validate Promotion & TransportPromotion
                    List<TransportPromotion> listTransportPromotion = draftOrder.TransportPromotionIds == null ? new List<TransportPromotion>() : _tPromotionRepository.FindByTransportPromotionIds(null, draftOrder.TransportPromotionIds);
                    ValidateResult validateResult = ValidateTransportPromotionCode(draftOrder.TransportPromotionIds, draftOrder.ListProduct, listProduct, listTransportPromotion);
                    if (!validateResult.Result) return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));

                    List<Promotion> listPromotion = draftOrder.PromotionIds == null ? new List<Promotion>() : _promotionRepository.FindByPromotionIds(null, draftOrder.PromotionIds);
                    validateResult = ValidatePromotionCode(draftOrder.PromotionIds, draftOrder.ListProduct, listProduct, listPromotion);
                    if (!validateResult.Result) return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));

                    List<string> shopids = draftOrder.ListProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();
                    List<Shop> listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
                    if (listShop == null || listShop.Count != shopids.Count) ResponseBadRequest(new CustomBadRequest(localizer("SHOP_NOTFOUND"), this.ControllerContext));
                    ResultCaculatorPriceOrder result = CaculatorPriceOrder(listTransportPromotion, draftOrder.ListProduct, listPromotion, listShop);
                    draftOrder.Price = result.TotalPriceOrder;
                    draftOrder.PromotionPrice = result.TotalPromotionPrice;
                    draftOrder.TransportPromotionPrice = result.TotalTransportPromotionPrice;
                    draftOrder.TransportPrice = result.TotalTransportPrice;
                }

                // Update value Account for DraftOrder
                if (string.IsNullOrEmpty(draftOrder.AccountName)) draftOrder.AccountName = account.Fullname;

                // Create or update cart
                if (string.IsNullOrEmpty(draftOrder.DraftOrderId)) // Create new cart
                {
                    var address = new Address();
                    var listAddress = _addressRepository.FindByIdAccount(account.AccountId);
                    if (listAddress != null && listAddress.Count > 0)
                    {
                        address = listAddress.Where(x => x.IsDefault == true).FirstOrDefault();
                        if (address == null && listAddress.Count > 1)
                        {
                            address = listAddress.FirstOrDefault();
                        }
                    }
                    if (string.IsNullOrEmpty(draftOrder.AddressId))
                    {
                        draftOrder.AddressId = address?.AddressId ?? "";
                        draftOrder.AddressAccount = address?.AddressAccount ?? "";
                    }
                    _draftOrderRepository.CreateDraftOrder(draftOrder);
                }
                else // Update cart
                {
                    _draftOrderRepository.UpdateDraftOrder(draftOrder);
                }

                DraftOrderDto draftOrderDto = _mapper.Map<DraftOrderDto>(draftOrder);
                foreach (var item in draftOrderDto.ListProduct)
                {
                    var detailProduct = listProduct.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    item.ProductNumber = detailProduct?.ProductNumber;
                    item.PurchasedNumber = detailProduct?.PurchasedNumber;
                    item.ProductRealPrice = detailProduct?.ProductRealPrice;
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = draftOrderDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/CreateOrUpdateOrder", $"Error user create or update order ", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/CreateOrUpdateOrder", ex);
            }
        }

        /// <summary>
        /// Save order (Tạo mới thông tin đơn hàng)
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Order</returns>
        // Post: api/shop/Order/SaveOrder
        [HttpPost("SaveOrder")]
        public IActionResult SaveOrder(DraftOrderSaveDto order)
        {
            try
            {
                if (string.IsNullOrEmpty(order.AccountId)) order.AccountId = GetUserIdAuth();
                Account account = _accountRepository.FindByIdAccount(order.AccountId);
                if (account == null) return ResponseBadRequest(new CustomBadRequest(localizer("BASE_USER_AUTH_NOT_FOUND"), this.ControllerContext));

                // Validate DraftOrder exits
                DraftOrder oldOrder = new DraftOrder();
                if (!string.IsNullOrEmpty(order.DraftOrderId))
                {
                    oldOrder = _draftOrderRepository.FindByIdDraftOrder(order.DraftOrderId);
                    if (oldOrder == null) return ResponseBadRequest(new CustomBadRequest(localizer("ORDER_NOTFOUND"), this.ControllerContext));
                }

                // Validate AddressAccount
                if (string.IsNullOrEmpty(order.AddressId) || string.IsNullOrEmpty(order.AddressAccount)) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_ADDRESS_NOTEXITS"), this.ControllerContext));

                // Validate ListProduct
                if (order.ListProduct == null || order.ListProduct.Count == 0) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_LIST_PRODUCT_NOTEXITS"), this.ControllerContext));

                // Caculator price Promotion & TransportPromotion if exits ListProduct
                List<Product> listProduct = _productRepository.FindByProductIds(String.Join(",", order.ListProduct.Select(x => x.ProductId)));

                List<TransportPromotion> listTransportPromotion = order.TransportPromotionIds == null ? new List<TransportPromotion>() : _tPromotionRepository.FindByTransportPromotionIds(order.TypePay, order.TransportPromotionIds);
                ValidateResult validateResult = ValidateTransportPromotionCode(order.TransportPromotionIds, order.ListProduct, listProduct, listTransportPromotion);
                if (!validateResult.Result) return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));

                List<Promotion> listPromotion = order.PromotionIds == null ? new List<Promotion>() : _promotionRepository.FindByPromotionIds(order.TypePay, order.PromotionIds);
                validateResult = ValidatePromotionCode(order.PromotionIds, order.ListProduct, listProduct, listPromotion);
                if (!validateResult.Result) return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));

                foreach (ProductOrder product in order.ListProduct)
                {
                    Product detailProduct = listProduct.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                    if (detailProduct == null || product.Quantity <= 0)
                    {
                        return ResponseBadRequest(new CustomBadRequest("Product: " + product.ProductId + " not exits or Quantity is empty", this.ControllerContext));
                    }
                    if (product.ProductName != detailProduct.ProductName)
                    {
                        return ResponseBadRequest(new CustomBadRequest("Product: " + product.ProductId + " ProductName is rowng", this.ControllerContext));
                    }
                    if (product.ProductPrice != detailProduct.ProductPrice)
                    {
                        return ResponseBadRequest(new CustomBadRequest("Product: " + product.ProductId + " ProductPrice is rowng", this.ControllerContext));
                    }

                    // Not update data from mobile, only update data from BE
                    product.ProductImages = detailProduct.ProductImages;
                    product.CategoryId = detailProduct.CategoryId;
                    product.SubCategoryId = detailProduct.SubCategoryId;
                    product.ShopId = detailProduct.ShopId;
                    product.PharmacistId = detailProduct.PharmacistId;
                    product.ProductName = detailProduct.ProductName;
                    product.ProductImages = detailProduct.ProductImages;
                    product.ProductPrice = (long)(detailProduct.ProductPrice ?? 0);

                    ProductOrder oldProduct = oldOrder.ListProduct.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                    if (oldProduct != null) oldOrder.ListProduct.Remove(oldProduct);
                }

                if (string.IsNullOrEmpty(order.AccountName)) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_ACCOUNT_NAME_EMPTY"), this.ControllerContext));
                if (string.IsNullOrEmpty(order.DraftOrderId)) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_DRAFTORDERID_NOTEXITS"), this.ControllerContext));
                List<string> shopids = order.ListProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();
                List<Shop> listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
                if (listShop == null || listShop.Count != shopids.Count) return ResponseBadRequest(new CustomBadRequest(localizer("SHOP_NOTFOUND"), this.ControllerContext));
                ResultCaculatorPriceOrder result = CaculatorPriceOrder(listTransportPromotion, order.ListProduct, listPromotion, listShop);
                foreach (Order newOrder in result.ListOrder)
                {
                    newOrder.AccountId = order.AccountId;
                    newOrder.AccountName = order.AccountName;
                    newOrder.AddressAccount = order.AddressAccount;
                    newOrder.TypePay = order.TypePay;
                }

                if (order.Price != result.TotalPriceOrder) return ResponseBadRequest(new CustomBadRequest("Order: " + result.TotalPriceOrder + " Price is rowng", this.ControllerContext));

                if (String.IsNullOrEmpty(order.TransactionId))
                {
                    _orderRepository.CreateListOrder(result.ListOrder);
                    _draftOrderRepository.UpdateTransactionId(order.DraftOrderId, result.ListOrder.First().TransactionId);
                }
                else
                {
                    var oldOrderTransactionId = _orderRepository.FindByIdTransactionId(order.TransactionId);
                    if (oldOrderTransactionId == null || oldOrderTransactionId.Count == 0)
                    {
                        return ResponseBadRequest(new CustomBadRequest("TransactionId: " + order.TransactionId + " " + localizer("ORDER_NOTFOUND"), this.ControllerContext));
                    }
                    if (oldOrderTransactionId.First().TypeStatusOrder == TypeStatusOrder.Successed)
                    {
                        return ResponseBadRequest(new CustomBadRequest("TransactionId: " + order.TransactionId + " " + localizer("ORDER_IS_SUCCESSED"), this.ControllerContext));
                    }
                    _orderRepository.DeleteSameTransactionId(order.TransactionId, order.AccountId);
                    _orderRepository.CreateListOrder(result.ListOrder);
                }
                SaveOrderResult resultSave = new SaveOrderResult();
                resultSave.ListOrder = _mapper.Map<List<OrderDto>>(result.ListOrder);
                DraftOrderDto draftOrder = _mapper.Map<DraftOrderDto>(order);
                resultSave.DraftOrder = draftOrder;
                return ResponseData(new { Timestamp = DateTimes.Now(), Data = resultSave });

            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/SaveOrder", $"Error user save order", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/SaveOrder", ex);
            }
        }

        public class ValidateModal
        {
            public List<ProductOrder> listProduct { get; set; }
            public List<string> transportPromotionIds { get; set; }
            public List<string> promotionIds { get; set; }
        }

        public class ResultValidateModal
        {
            public bool Result { get; set; } = true;
            public long TotalPriceOrder { get; set; }
            public long TotalPromotionPrice { get; set; }
            public long TotalTransportPromotionPrice { get; set; }
            public long TotalTransportPrice { get; set; }
        }

        /// <summary>
        /// Validate Promotion (Kiểm tra danh sách sản phẩm thuộc chương trình khuyến mãi giảm giá)
        /// </summary>
        /// <param name="validatePromotion"></param>
        /// <param name="typePay"></param>
        /// <returns>Order</returns>
        // Post: api/shop/Order/ValidatePromotion
        [HttpPost("ValidatePromotion")]
        public IActionResult ValidatePromotion(ValidateModal validatePromotion, TypePay typePay)
        {
            try
            {
                ValidateResult validateResult = new ValidateResult();
                if (validatePromotion == null || validatePromotion.listProduct == null || validatePromotion.listProduct.Count == 0)
                {
                    validateResult.Message = "listProduct is empty";
                    return ResponseData(validateResult);
                }
                if (validatePromotion.promotionIds == null) validatePromotion.promotionIds = new List<string>();
                if (validatePromotion.transportPromotionIds == null) validatePromotion.transportPromotionIds = new List<string>();
                List<Product> listProducts = _productRepository.FindByProductIds(String.Join(",", validatePromotion.listProduct.Select(x => x.ProductId)));
                List<Promotion> listPromotion = _promotionRepository.FindByPromotionIds(typePay, validatePromotion.promotionIds);
                List<TransportPromotion> listTransportPromotion = _tPromotionRepository.FindByTransportPromotionIds(typePay, validatePromotion.transportPromotionIds);
                validateResult = ValidatePromotionCode(validatePromotion.promotionIds, validatePromotion.listProduct, listProducts, listPromotion);
                if (!validateResult.Result)
                    return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));
                validateResult = ValidateTransportPromotionCode(validatePromotion.transportPromotionIds, validatePromotion.listProduct, listProducts, listTransportPromotion);
                if (!validateResult.Result)
                    return ResponseBadRequest(new CustomBadRequest(validateResult.Message, this.ControllerContext));
                List<string> shopids = validatePromotion.listProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();
                List<Shop> listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
                if (listShop == null || listShop.Count != shopids.Count) ResponseBadRequest(new CustomBadRequest(localizer("SHOP_NOTFOUND"), this.ControllerContext));
                ResultValidateModal resultValidateModal = new ResultValidateModal();
                var resultCaculator = CaculatorPriceOrder(listTransportPromotion, validatePromotion.listProduct, listPromotion, listShop);
                resultValidateModal.TotalPriceOrder = resultCaculator.TotalPriceOrder;
                resultValidateModal.TotalPromotionPrice = resultCaculator.TotalPromotionPrice;
                resultValidateModal.TotalTransportPromotionPrice = resultCaculator.TotalTransportPromotionPrice;
                resultValidateModal.TotalTransportPrice = resultCaculator.TotalTransportPrice;
                return ResponseData(resultValidateModal);
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Order/ValidatePromotion", $"Error user validate promotion ", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Order/ValidatePromotion", ex);
            }
        }

        private ValidateResult ValidatePromotionCode(List<string> codes, List<ProductOrder> listProductOrder, List<Product> listProducts, List<Promotion> listPromotion)
        {
            var validateResult = new ValidateResult() { };


            if (listProductOrder == null || listProductOrder.Count == 0)
            {
                validateResult.Message = "listProduct not exits";
                return validateResult;
            }

            foreach (var product in listProductOrder)
            {
                var detailProduct = listProducts.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                if (!detailProduct.SellOver.Value || detailProduct.PurchasedNumber + product.Quantity >= detailProduct.ProductNumber)
                {
                    validateResult.Message = "Product: " + product.ProductId + " is sold out";
                    return validateResult;
                }
                if (detailProduct == null || product.Quantity <= 0)
                {

                    validateResult.Message = "Product: " + product.ProductId + " not exits or Quantity is empty";
                    return validateResult;
                }
                if (detailProduct.ProductPrice != product.ProductPrice)
                {
                    validateResult.Message = "Product: " + product.ProductId + " Price is rowng";
                    return validateResult;
                }
                product.ShopId = detailProduct.ShopId;
            }

            //var x = listPromotion.GroupBy(p => p.ShopId).Where(group => group.Select(cs => cs.PromotionId).Count() > 1).Select(group => new { ShopId = group.Key, Items = group.Select(cs => cs.PromotionId).ToList() });

            //var listInvalidate = "";
            //foreach (var item in x)
            //{
            //    if (listInvalidate == "") listInvalidate += string.Join(",", item.Items);
            //    listInvalidate += "," + string.Join(",", item.Items);
            //}

            //if (listInvalidate != "")
            //{
            //    validateResult.Message = "PromotionId: " + listInvalidate + " can't use mutiple promotion in same shop";
            //    return validateResult;
            //}
            if (codes != null && codes.Count > 0)
            {
                var duplicate = codes.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicate != null && duplicate.Count != 0)
                {
                    validateResult.Message = "PromotionId " + String.Join(",", duplicate) + " is duplicate";
                    return validateResult;
                }
                foreach (var promotionId in codes)
                {
                    var tPromotion = listPromotion.Where(x => x.PromotionId == promotionId).FirstOrDefault();
                    if (tPromotion == null)
                    {
                        validateResult.Message = "PromotionId: " + promotionId + localizer("TYPE_PAY_NOT_MATCH_PROMOTION");
                        return validateResult;
                    }
                    var listProductInShop = listProducts.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (listProductInShop == null || listProductInShop.Count == 0)
                    {
                        validateResult.Message = "PromotionId: " + promotionId + " can't use with list Product";
                        return validateResult;
                    }
                    var listProductOrderInShop = listProductOrder.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (tPromotion.MinBuget > 0 && listProductOrderInShop.Sum(x => x.ProductPrice * x.Quantity) < tPromotion.MinBuget)
                    {
                        validateResult.Message = "PromotionId: " + promotionId + " can't use by low than MinBuget";
                        return validateResult;
                    }
                    if (tPromotion.PromotionQuantity <= tPromotion.QuantityUsed)
                    {
                        validateResult.Message = "PromotionId: " + tPromotion.PromotionId + " is used out";
                        return validateResult;
                    }
                }
            }

            validateResult.Result = true;

            return validateResult;
        }

        private ValidateResult ValidateTransportPromotionCode(List<string> codes, List<ProductOrder> listProductOrder, List<Product> listProducts, List<TransportPromotion> listPromotion)
        {
            var validateResult = new ValidateResult() { };


            if (listProductOrder == null || listProductOrder.Count == 0)
            {
                validateResult.Message = "listProduct not exits";
                return validateResult;
            }

            foreach (var product in listProductOrder)
            {
                var detailProduct = listProducts.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                if (detailProduct == null || product.Quantity <= 0)
                {
                    validateResult.Message = "Product: " + product.ProductId + " not exits or Quantity is empty";
                    return validateResult;
                }
                product.ShopId = detailProduct.ShopId;
            }

            //var x = listPromotion.GroupBy(p => p.ShopId).Where(group => group.Select(cs => cs.TransportPromotionId).Count() > 1).Select(group => new { ShopId = group.Key, Items = group.Select(cs => cs.TransportPromotionId).ToList() });

            //var listInvalidate = "";
            //foreach (var item in x)
            //{
            //    if (listInvalidate == "") listInvalidate += string.Join(",", item.Items);
            //    listInvalidate += "," + string.Join(",", item.Items);
            //}

            //if (listInvalidate != "")
            //{
            //    validateResult.Message = "PromotionId: " + listInvalidate + " can't use mutiple promotion in same shop";
            //    return validateResult;
            //}
            if (codes != null && codes.Count > 0)
            {
                var duplicate = codes.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicate != null && duplicate.Count != 0)
                {

                    validateResult.Message = "TransportPromotionId " + String.Join(",", duplicate) + " is duplicate";
                    return validateResult;
                }
                foreach (var promotionId in codes)
                {
                    var tPromotion = listPromotion.Where(x => x.TransportPromotionId == promotionId).FirstOrDefault();
                    if (tPromotion == null)
                    {
                        validateResult.Message = "TransportPromotionId: " + promotionId + localizer("TYPE_PAY_NOT_MATCH_PROMOTION");
                        return validateResult;
                    }

                    var listProductInShop = listProducts.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (listProductInShop == null || listProductInShop.Count == 0)
                    {
                        validateResult.Message = "TransportPromotionId: " + promotionId + " can't use with list Product";
                        return validateResult;
                    }
                    var listProductOrderInShop = listProductOrder.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (tPromotion.MinBuget > 0 && listProductOrderInShop.Sum(x => x.ProductPrice * x.Quantity) < tPromotion.MinBuget)
                    {
                        validateResult.Message = "TransportPromotionId: " + promotionId + " can't use by low than MinBuget";
                        return validateResult;
                    }

                    if (tPromotion.TransportPromotionQuantity <= tPromotion.QuantityUsed)
                    {
                        validateResult.Message = "PromotionId: " + tPromotion.TransportPromotionId + " is used out";
                        return validateResult;
                    }
                }
            }
            validateResult.Result = true;
            return validateResult;
        }

        public class ResultCaculatorPriceOrder
        {
            public List<Order> ListOrder { get; set; }
            public long TotalPriceOrder { get; set; }
            public long TotalPromotionPrice { get; set; }
            public long TotalTransportPromotionPrice { get; set; }
            public long TotalTransportPrice { get; set; }
        }

        private ResultCaculatorPriceOrder CaculatorPriceOrder(List<TransportPromotion> listTransportPromotion, List<ProductOrder> listProduct, List<Promotion> listPromotion, List<Shop> listshop)
        {
            var shopIds = listProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();

            var ListOrder = new List<Order>();
            var transactionId = Guid.NewGuid().ToString();
            var ListUpdatePromotion = new List<string>();
            var ListUpdateTPromotion = new List<string>();
            var ListUpdateProduct = new List<string>();
            var listShop = _shopRepository.FindByIdShops(String.Join(",", shopIds));
            foreach (var shopId in shopIds)
            {
                var newOrder = new Order();
                var listProductOrder = listProduct.Where(x => x.ShopId == shopId).ToList();
                newOrder.ListProduct = listProductOrder;
                long totalPrice = 0;
                foreach (var productOrder in listProductOrder)
                {
                    totalPrice += productOrder.ProductPrice * productOrder.Quantity;
                }
                var currentPrice = totalPrice;
                var promotionOrder = listPromotion?.Where(x => x.ShopId == shopId).ToList();
                if (promotionOrder != null)
                {
                    foreach (var promotion in promotionOrder)
                    {
                        if (promotion.TypeDiscount == TypePromotionsDiscount.TypePromotionsDiscount_2)
                            totalPrice -= (promotion.Price ?? 0);
                        else
                        {
                            if (promotion.MaxPriceDiscount > 0)
                            {
                                var PriceDiscount = ((totalPrice * promotion.Discount) / 100);
                                totalPrice -= PriceDiscount > promotion.MaxPriceDiscount ? (promotion.MaxPriceDiscount ?? 0) : ((totalPrice * (promotion.Discount ?? 0)) / 100);
                            }
                            else
                            {
                                totalPrice -= ((totalPrice * (promotion.Discount ?? 0)) / 100);
                            }

                        }
                        newOrder.Promotion.Add(promotion);
                        newOrder.PromotionIds.Add(promotion.PromotionId);
                    }

                }
                if (totalPrice < 0) totalPrice = 0;
                newOrder.PromotionPrice = currentPrice - totalPrice;
                var transportPromotion = listTransportPromotion?.Where(x => x.ShopId == shopId).ToList();
                var transportPrice = listShop.Where(x => x.ShopId == shopId).FirstOrDefault().TransportPrice;
                long totalTransportPrice = transportPrice;
                if (transportPromotion != null && transportPromotion.Count > 0)
                    foreach (var tPromotion in transportPromotion)
                    {
                        if (tPromotion.TypeDiscount == TypeTransportPromotionsDiscount.TypeTransportPromotionsDiscount_2)
                        {
                            totalTransportPrice -= (tPromotion.Price ?? 0);
                        }
                        else
                        {
                            if (tPromotion.MaxPriceDiscount > 0)
                            {
                                var PriceDiscount = ((transportPrice * tPromotion.Discount) / 100);
                                totalTransportPrice -= PriceDiscount > tPromotion.MaxPriceDiscount ? (tPromotion.MaxPriceDiscount ?? 0) : ((transportPrice * (tPromotion.Discount ?? 0)) / 100);
                            }
                            else
                            {
                                totalTransportPrice -= ((transportPrice * (tPromotion.Discount ?? 0)) / 100);
                            }
                        }
                        if (totalTransportPrice < 0) totalTransportPrice = 0;
                        newOrder.TransportPromotion.Add(tPromotion);
                        newOrder.TransportPromotionIds.Add(tPromotion.TransportPromotionId);
                    }
                newOrder.TransportPrice = totalTransportPrice;
                newOrder.TransportPromotionPrice = transportPrice - totalTransportPrice;
                newOrder.TransportPromotionPrice = newOrder.TransportPromotionPrice ?? 0;
                newOrder.Price = totalPrice + (newOrder.TransportPrice ?? 0);
                newOrder.TransactionId = transactionId;
                newOrder.ShopId = shopId;
                newOrder.Type = TypeEnum.Actived;
                newOrder.TypeStatusOrder = TypeStatusOrder.Pending;
                ListOrder.Add(newOrder);
            }
            ResultCaculatorPriceOrder resultCaculatorPriceOrder = new ResultCaculatorPriceOrder()
            {
                ListOrder = ListOrder,
                TotalPriceOrder = ListOrder.Sum(x => x.Price),
                TotalPromotionPrice = ListOrder.Sum(x => x.PromotionPrice ?? 0),
                TotalTransportPromotionPrice = ListOrder.Sum(x => x.TransportPromotionPrice ?? 0),
                TotalTransportPrice = ListOrder.Sum(x => x.TransportPrice ?? 0),
            };
            return resultCaculatorPriceOrder;
        }

        public class TestPaymentModel
        {
            [DefaultValue("")]
            public string TransactionId { get; set; }
            [DefaultValue("")]
            public long Amount { get; set; }
            [DefaultValue(TypeStatusOrder.Successed)]
            public TypeStatusOrder TypeStatusOrder { get; set; } = TypeStatusOrder.Pending;

            [DefaultValue(TypePay.Other)]
            public TypePay TypePay { get; set; } = TypePay.Other;
        }

        /// <summary>
        /// Test Payment
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("TestPayment")]
        [AllowAnonymous]
        public IActionResult TestPayment(TestPaymentModel model)
        {
            try
            {
                var result = TestPaymentMethod(model);

                if (!result.Result)
                    return ResponseBadRequest(new CustomBadRequest(result.Message, this.ControllerContext));

                var oldOrder = _draftOrderRepository.FindByTransactionId(model.TransactionId);
                if (oldOrder != null)
                {
                    _productRepository.UpdatePurchasedNumber(oldOrder.ListProduct);
                    if (oldOrder.PromotionIds != null)
                        _promotionRepository.UpdateUsed(oldOrder.PromotionIds);
                    if (oldOrder.TransportPromotionIds != null)
                        _tPromotionRepository.UpdateUsed(oldOrder.TransportPromotionIds);

                    if (oldOrder.ListProduct.Count > 0)
                    {
                        var listPromotion = new List<Promotion>();
                        var listTransportPromotion = new List<TransportPromotion>();
                        var shopids = oldOrder.ListProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();
                        List<Shop> listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
                        ResultCaculatorPriceOrder resultUpdate = CaculatorPriceOrder(listTransportPromotion, oldOrder.ListProduct, listPromotion, listShop);
                        oldOrder.Price = resultUpdate.TotalPriceOrder;
                        oldOrder.PromotionPrice = resultUpdate.TotalPromotionPrice;
                        oldOrder.TransportPromotionPrice = resultUpdate.TotalTransportPromotionPrice;
                        oldOrder.TransportPrice = resultUpdate.TotalTransportPrice;
                        oldOrder.TransportPromotionIds = new List<string>();
                        oldOrder.PromotionIds = new List<string>();
                        oldOrder.TransactionId = null;
                        _draftOrderRepository.UpdateDraftOrder(oldOrder);
                    }
                    else
                    {
                        oldOrder.ListProduct = new List<ProductOrder>();
                        oldOrder.Price = 0;
                        oldOrder.PromotionPrice = 0;
                        oldOrder.TransportPromotionPrice = 0;
                        oldOrder.TransportPrice = 0;
                        oldOrder.TransportPromotionIds = new List<string>();
                        oldOrder.PromotionIds = new List<string>();
                        oldOrder.TransactionId = null;
                        _draftOrderRepository.UpdateDraftOrderById(oldOrder);
                    }
                    DraftOrderDto draftOrder = _mapper.Map<DraftOrderDto>(oldOrder);
                    return ResponseData(new { Timestamp = DateTimes.Now(), Data = draftOrder });
                }
                return ResponseData(new { Timestamp = DateTimes.Now(), Result = true });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private ValidateResult TestPaymentMethod(TestPaymentModel model)
        {
            ValidateResult result = new ValidateResult();
            try
            {
                List<Order> orders = _orderRepository.FindByIdTransactionId(model.TransactionId);
                if (orders == null || orders.Count == 0)
                {
                    result.Message = localizer("ORDER_NOTFOUND");
                    return result;
                }

                if (orders.FirstOrDefault().TypePay != model.TypePay)
                {
                    result.Message = localizer("TYPE_PAY_NOT_MATCH");
                    return result;
                }

                var totalPrice = orders.Sum(x => x.Price);
                if (totalPrice != model.Amount)
                {
                    result.Message = localizer("AMOUNT_NOT_MATCH");
                    return result;
                }

                _orderRepository.UpdateListOrder(orders, model.TypeStatusOrder);
                result.Result = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

    }
}

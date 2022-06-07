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
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;
using static Sophie.Controllers.API.OrderController;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class CreateDraftOrderModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly IDraftOrderRepository _draftOrderRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ITransportPromotionRepository _transportPromotionRepository;
        private readonly IAddressRepository _addressRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public DraftOrder? DraftOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Product> ListProduct { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Account> ListAccount { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Promotion> ListPromotion { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<TransportPromotion> ListTransportPromotion { get; set; }

        public CreateDraftOrderModel(IConfiguration config, IMapper mapper, IDraftOrderRepository draftOrderRepository, IShopRepository shopRepository
            , IProductRepository productRepository, IAccountRepository accountRepository, ITransportPromotionRepository transportPromotionRepository,
            IPromotionRepository promotionRepository, IAddressRepository addressRepository)
        {
            _config = config;
            _mapper = mapper;
            _draftOrderRepository = draftOrderRepository;
            _shopRepository = shopRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
            _transportPromotionRepository = transportPromotionRepository;
            _promotionRepository = promotionRepository;
            _addressRepository = addressRepository;
        }

        public void OnGet(string draftOrderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(draftOrderId))
            {
                DraftOrder = _draftOrderRepository.FindByIdDraftOrder(draftOrderId);
            }
            if (DraftOrder == null)
            {
                DraftOrder = new DraftOrder()
                {
                    PharmacistId = userId,
                    Type = TypeEnum.Actived,
                };
            }
            GetDataCreate();
        }

        private void GetDataCreate()
        {
            ListProduct = _productRepository.FindAll();
            ListAccount = _accountRepository.ListAccount(0, int.MaxValue);
            Paging paging = new Paging()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
            };
            ListTransportPromotion = _transportPromotionRepository.ListPromotionActive(paging).Result;
            ListPromotion = _promotionRepository.ListPromotionActive(paging).Result;
        }

        public IActionResult OnPostCreate(DraftOrder model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            DraftOrder draftOrderAccount = _draftOrderRepository.FindByAccountId(model.AccountId);

            if (draftOrderAccount != null && draftOrderAccount.DraftOrderId != model.DraftOrderId)
            {
                StatusMessage = "Khách hàng này đã tồn tại 1 draft order";
                GetDataCreate();
                return Page();
            }
            model.ListProduct = model.ListProduct.Where(x => x.Quantity > 0).ToList();
            if (model.ListProduct.Count == 0)
            {
                StatusMessage = "List ListProduct is empty";
                GetDataCreate();
                return Page();
            }
            if (!string.IsNullOrEmpty(model.AccountId))
            {
                var account = _accountRepository.FindByIdAccount(model.AccountId);
                var addresses = _addressRepository.FindByIdAccount(model.AccountId).Where(x => x.IsDefault == true).FirstOrDefault();
                if (account != null)
                {
                    model.AccountName = account.Fullname;
                    model.AddressAccount = addresses?.AddressAccount;
                    model.AddressId = addresses?.AddressId;
                }
            }
            var result = ValidateAndCaculator(model);
            if (!result.Result)
            {
                StatusMessage = result.Message;
                GetDataCreate();
                return Page();
            }
            model.ListProduct = result.ListProduct;
            model.Price = result.TotalPriceOrder;
            model.TransportPrice = result.TotalTransportPrice;
            model.TransportPromotionPrice = result.TotalTransportPromotionPrice;
            model.PromotionPrice = result.TotalPromotionPrice;
            if (string.IsNullOrEmpty(model.DraftOrderId))
            {
                model.PharmacistId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _draftOrderRepository.CreateDraftOrder(model);
                StatusMessage = "DraftOrder created successfully";
            }
            else
            {
                _draftOrderRepository.UpdateDraftOrder(model);
                StatusMessage = "DraftOrder edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/DraftOrder/ListDraftOrder");
        }

        public IActionResult OnGetDelete(string draftOrderId)
        {
            DraftOrder? item = _draftOrderRepository.FindByIdDraftOrder(draftOrderId);
            if (item == null)
            {
                StatusMessage = "Error: DraftOrder not found";
            }
            else
            {
                _draftOrderRepository.DeleteDraftOrder(item.DraftOrderId);

                StatusMessage = "DraftOrder deleted successfully";
                return LocalRedirect($"~/Admin/ShopPage/DraftOrder/ListDraftOrder");
            }
            return Page();
        }

        public IActionResult OnPostCaculator(DraftOrder model)
        {
            return new JsonResult(ValidateAndCaculator(model));
        }
        public ResultCaculatorPriceOrder ValidateAndCaculator(DraftOrder model)
        {
            ResultCaculatorPriceOrder Result = new ResultCaculatorPriceOrder();
            if (model == null || model.ListProduct == null)
            {
                Result.Message = "listProduct is empty";
                return Result;
            }
            if (model.PromotionIds == null) model.PromotionIds = new List<string>();
            if (model.TransportPromotionIds == null) model.TransportPromotionIds = new List<string>();
            List<Product> listProducts = _productRepository.FindByProductIds(String.Join(",", model.ListProduct.Select(x => x.ProductId)));
            List<Promotion> listPromotion = _promotionRepository.FindByPromotionIds(null, model.PromotionIds);
            List<TransportPromotion> listTransportPromotion = _transportPromotionRepository.FindByTransportPromotionIds(null, model.TransportPromotionIds);
            ValidateResult validateResult = ValidatePromotionCode(model.PromotionIds, model.ListProduct, listProducts, listPromotion);
            if (!validateResult.Result)
            {
                Result.Message = validateResult.Message;
                return Result;
            }
            validateResult = ValidateTransportPromotionCode(model.TransportPromotionIds, model.ListProduct, listProducts, listTransportPromotion);
            if (!validateResult.Result)
            {
                Result.Message = validateResult.Message;
                return Result;
            }
            var shopids = model.ListProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();
            var listShop = _shopRepository.FindByIdShops(String.Join(",", shopids));
            if (listShop == null || listShop.Count != shopids.Count)
            {
                Result.Message = "SHOP NOT FOUND";
                return Result;
            }
            List<TransportPrice> ListTransportPrice = new List<TransportPrice>();
            foreach (var shop in listShop)
            {
                ListTransportPrice.Add(new TransportPrice
                {
                    ShopId = shop.ShopId,
                    Price = shop.TransportPrice
                });
            }
            foreach (var product in model.ListProduct)
            {
                var detailProduct = listProducts.FirstOrDefault(x => x.ProductId == product.ProductId);
                if (string.IsNullOrEmpty(product.ProductName)) product.ProductName = detailProduct.ProductName;
                product.ProductImages = detailProduct.ProductImages;
                if (product.CategoryId == null) product.CategoryId = detailProduct.CategoryId;
                if (product.SubCategoryId == null) product.SubCategoryId = detailProduct.SubCategoryId;
            }
            return CaculatorPriceOrder(listTransportPromotion, model.ListProduct, listPromotion, ListTransportPrice);
        }

        public class ValidateResult
        {
            public string Message { get; set; }
            public bool Result { get; set; }
        }

        public class ResultCaculatorPriceOrder
        {
            public List<Order> ListOrder { get; set; }
            public List<ProductOrder>? ListProduct { get; set; }
            public long TotalPriceOrder { get; set; }
            public long TotalPromotionPrice { get; set; }
            public long TotalTransportPromotionPrice { get; set; }
            public long TotalTransportPrice { get; set; }
            public string Message { get; set; }
            public bool Result { get; set; }
        }

        public class ResultValidateModal
        {
            public bool Result { get; set; } = true;
            public long TotalPriceOrder { get; set; }
            public long TotalPromotionPrice { get; set; }
            public long TotalTransportPromotionPrice { get; set; }
            public long TotalTransportPrice { get; set; }
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

            var x = listPromotion.GroupBy(p => p.ShopId).Where(group => group.Select(cs => cs.PromotionId).Count() > 1).Select(group => new { ShopId = group.Key, Items = group.Select(cs => cs.PromotionId).ToList() });

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
                        validateResult.Message = "PromotionId: " + promotionId + " can't find this promotion";
                        return validateResult;
                    }
                    var listProductInShop = listProducts.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (listProductInShop == null || listProductInShop.Count == 0)
                    {
                        validateResult.Message = "PromotionName: " + tPromotion.PromotionName + " can't find product same shop";
                        return validateResult;
                    }
                    var listProductOrderInShop = listProductOrder.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (tPromotion.MinBuget > 0 && listProductOrderInShop.Sum(x => x.ProductPrice * x.Quantity) < tPromotion.MinBuget)
                    {
                        validateResult.Message = "PromotionName: " + tPromotion.PromotionName + " can't use by low than MinBuget";
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
                        validateResult.Message = "TransportPromotionId: " + promotionId + " can't use with list Product";
                        return validateResult;
                    }

                    var listProductInShop = listProducts.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (listProductInShop == null || listProductInShop.Count == 0)
                    {
                        validateResult.Message = "TransportPromotionName: " + tPromotion.TransportPromotionName + " can't fint product same shop";
                        return validateResult;
                    }
                    var listProductOrderInShop = listProductOrder.Where(x => x.ShopId == tPromotion.ShopId).ToList();
                    if (tPromotion.MinBuget > 0 && listProductOrderInShop.Sum(x => x.ProductPrice * x.Quantity) < tPromotion.MinBuget)
                    {
                        validateResult.Message = "TransportPromotionName: " + tPromotion.TransportPromotionName + " can't use by low than MinBuget";
                        return validateResult;
                    }
                }
            }
            validateResult.Result = true;
            return validateResult;
        }

        private ResultCaculatorPriceOrder CaculatorPriceOrder(List<TransportPromotion> listTransportPromotion, List<ProductOrder> listProduct, List<Promotion> listPromotion, List<TransportPrice> ListTransportPrice)
        {
            var shopIds = listProduct.GroupBy(x => x.ShopId).Select(x => x.Key).ToList();

            var ListOrder = new List<Order>();
            var transactionId = Guid.NewGuid().ToString();
            var ListUpdatePromotion = new List<string>();
            var ListUpdateTPromotion = new List<string>();
            var ListUpdateProduct = new List<string>();
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
                var transportPrice = ListTransportPrice?.Where(x => x.ShopId == shopId).FirstOrDefault();
                if (transportPrice != null)
                {
                    long totalTransportPrice = transportPrice.Price;
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
                                    var PriceDiscount = ((transportPrice.Price * tPromotion.Discount) / 100);
                                    totalTransportPrice -= PriceDiscount > tPromotion.MaxPriceDiscount ? (tPromotion.MaxPriceDiscount ?? 0) : ((transportPrice.Price * (tPromotion.Discount ?? 0)) / 100);
                                }
                                else
                                {
                                    totalTransportPrice -= ((transportPrice.Price * (tPromotion.Discount ?? 0)) / 100);
                                }
                            }
                            if (totalTransportPrice < 0) totalTransportPrice = 0;
                            newOrder.TransportPromotion.Add(tPromotion);
                            newOrder.TransportPromotionIds.Add(tPromotion.TransportPromotionId);
                        }
                    newOrder.TransportPrice = totalTransportPrice;
                    newOrder.TransportPromotionPrice = transportPrice.Price - totalTransportPrice;
                }
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
                Result = true,
                ListProduct = listProduct,
            };
            return resultCaculatorPriceOrder;
        }

    }
}

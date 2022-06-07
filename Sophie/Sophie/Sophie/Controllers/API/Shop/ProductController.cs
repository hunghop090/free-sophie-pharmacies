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
using Microsoft.Extensions.Logging;
using Sophie.Model;
using App.Core.Units;
using Sophie.Units;
using System.ComponentModel;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API_ACCOUNT)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v5")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.Account, IsAnd = false)]
    public class ProductController : BaseAPIController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(ProductController));
        private readonly ILogger<ProductController> _logger;

        private readonly IConfiguration _config;
        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IConfiguration config, IProductRepository productRepository)
        {
            _logger = logger;
            _config = config;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Search list product (Tìm kiếm danh sách sản phẩm)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List product</returns>
        // Get: api/shop/Product/SearchProductList
        [HttpGet("SearchProductList")]
        public IActionResult SearchProductList(string? search = "", int skip = 0, int limit = 99)
        {
            try
            {
                Paging paging = new Paging()
                {
                    search = search,
                    PageIndex = skip / (limit == 0 ? 1 : limit),
                    PageSize = limit,
                    sortName = "Created",
                    sort = "desc"
                };
                AmazonUploader AU = new AmazonUploader(_config);
                PagingResult<Product> listProduct = _productRepository.ListSearchProduct(paging);
                List<ProductInListDto> listProductDto = _mapper.Map<List<ProductInListDto>>(listProduct.Result);
                foreach (var product in listProductDto)
                {
                    if (product.ProductImages != null)
                    {
                        var ListImage = new List<string>();
                        foreach (var image in product.ProductImages)
                        {
                            ListImage.Add(AU.GetUrlImage(image, "/promotions/" + product.ProductId + "/"));
                        };
                        product.ProductImages = ListImage;
                    }
                }

                //return ResponseData(new PagingResult<ProductInListDto> { Result = listProductDto, Total = listProduct.Total });
                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listProductDto, Total = listProductDto.Count });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Product/SearchProductList", $"Error user search list product", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Product/SearchProductList", ex);
            }
        }

        public class FilterProduct
        {
            [DefaultValue("c16366aa-3ccc-4bf5-869a-3cef68e8c54e")]
            public string? CategoryId { get; set; }

            [DefaultValue("")]
            public string? SubCategoryId { get; set; }

            [DefaultValue(100000)]
            public long? MinPrice { get; set; }

            [DefaultValue(100000000)]
            public long? MaxPrice { get; set; }

            [DefaultValue(false)]
            public bool? IsSale { get; set; }

            [DefaultValue(true)]
            public bool? IsRelated { get; set; }
        }

        public enum SortType
        {
            desc,
            asc,
        }

        public enum SortFilterProduct
        {
            Created,
            PurchasedNumber,
            ProductPrice
        }

        /// <summary>
        /// Get list product (Lấy danh sách sản phẩm theo bộ lọc)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortName"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>List product</returns>
        // Post: api/shop/Product/FilterProductList
        [HttpPost("FilterProductList")]
        public IActionResult FilterProductList([FromBody] FilterProduct filter, SortType? sort = SortType.desc, SortFilterProduct? sortName = SortFilterProduct.Created, int skip = 0, int limit = 99)
        {
            try
            {
                AmazonUploader AU = new AmazonUploader(_config);
                PagingResult<Product> listProduct = _productRepository.ListProductFilter(filter, sort, sortName, limit, (skip / (limit == 0 ? 1 : limit)));
                List<ProductInListDto> listProductDto = _mapper.Map<List<ProductInListDto>>(listProduct.Result);
                foreach (var product in listProductDto)
                {
                    if (product.ProductImages != null)
                    {
                        var ListImage = new List<string>();
                        foreach (var image in product.ProductImages)
                        {
                            ListImage.Add(AU.GetUrlImage(image, "/products/" + product.ProductId + "/"));
                        };
                        product.ProductImages = ListImage;
                    }
                }

                //return ResponseData(new PagingResult<ProductInListDto> { Result = listProductDto, Total = listProduct.Total });
                return ResponseData(new { Timestamp = DateTimes.Now(), Data = listProductDto, Total = listProductDto.Count });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Product/FilterProductList", $"Error user filter list product", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Product/FilterProductList", ex);
            }
        }

        /// <summary>
        /// Get product by id (Lấy thông tin chi tiết sản phẩm theo productId)
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>Product</returns>
        // GET: api/shop/Product/GetById
        [HttpGet("GetById")]
        public IActionResult GetById(string productId)
        {
            try
            {
                AmazonUploader AU = new AmazonUploader(_config);
                ProductDto productDto = _mapper.Map<ProductDto>(_productRepository.FindByIdProduct(productId));
                if (productDto == null) return ResponseBadRequest(new CustomBadRequest(localizer("PRODUCT_NOTFOUND"), this.ControllerContext));

                if (productDto.ProductImages != null)
                {
                    var ListImage = new List<string>();
                    foreach (var image in productDto.ProductImages)
                    {
                        ListImage.Add(AU.GetUrlImage(image, "/products/" + productDto.ProductId + "/"));
                    };
                    productDto.ProductImages = ListImage;
                }

                return ResponseData(new { Timestamp = DateTimes.Now(), Data = productDto });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Product/GetById", $"Error user get detail product", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Product/GetById", ex);
            }
        }
    }
}

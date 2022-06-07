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
using Sophie.Resource.Dtos.Shop;
using Microsoft.Extensions.Logging;
using AutoMapper;
using App.Core.Units.Log4Net;
using AutoMapper.Configuration;
using Microsoft.Extensions.Localization;
using App.SharedLib.Repository.Interface;
using Amazon.Util.Internal.PlatformServices;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Sophie.Model;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API_ACCOUNT)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v5")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.Account, IsAnd = false)]
    public class CategoryController : BaseAPIController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(CategoryController));
        private readonly ILogger<CategoryController> _logger;

        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get list category (Lấy tấc cả danh sách danh mục lv1 + lv2)
        /// </summary>
        /// <returns>List category</returns>
        // GET: api/shop/Category/Logout
        [HttpGet("ListCategory")]
        public IActionResult ListCategory()
        {
            try
            {
                List<Category> listCategory = _categoryRepository.ListCategoryActive();
                List<CategoryDto> listCategoryDto = _mapper.Map<List<CategoryDto>>(listCategory);
                
                return ResponseData(listCategoryDto);
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Category/ListCategory", $"Error user get list category", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Category/ListCategory", ex);
            }
        }

        /// <summary>
        /// Get category by id (Lấy danh sách danh mục lv2 theo id danh mục lv1)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>Category</returns>
        // GET: api/shop/Category/Logout
        [HttpGet()]
        public IActionResult Index(string categoryId)
        {
            try
            {
                Category category = _categoryRepository.FindByIdCategory(categoryId);
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);

                return ResponseData(categoryDto);
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Category/Index", $"Error user get category by id", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Category/Index", ex);
            }
        }
    }
}

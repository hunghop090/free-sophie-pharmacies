using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class ListCategoryModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<Category>? ListCategorys { get; set; }

        public ListCategoryModel(IConfiguration config, IMapper mapper, ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _config = config;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public Task<JsonResult> OnGetList()
        {
            //foreach (var item in Request.Query.ToArray())
            //{
            //    Logs.debug("OnGetList query: " + item.Key + " = " + item.Value);
            //}
            int draw = int.Parse(Request.Query["draw"]);
            int start = int.Parse(Request.Query["start"]);
            int length = int.MaxValue;
            string search = Request.Query["search[value]"];
            string sortName = Request.Query["order[0][column]"];
            string sort = Request.Query["order[0][dir]"];

            long total = 0;
            int page = 0;

            List<Category> listData = new List<Category>();
            if (!string.IsNullOrEmpty(search))
            {
                List<Category> listSearch = _categoryRepository.ListCategory(0, int.MaxValue).FindAll(item => item.CategoryName.ToLower().IndexOf(search.ToLower()) >= 0);
                total = listSearch.Count;
                page = start / length;
                listData = listSearch.Skip(page * length).Take(length).ToList();
            }
            else
            {
                total = _categoryRepository.TotalCategory();
                page = start / length;
                listData = _categoryRepository.ListCategory(page, length);
            }
            return Task.FromResult(new JsonResult(new { draw = draw, recordsTotal = total, recordsFiltered = total, data = listData }));
        }
    }
}

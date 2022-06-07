using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Units;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class EditCategoryModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Category? Category { get; set; }

        [BindProperty]
        public SubCategory? SubCategory { get; set; }

        [BindProperty]
        public List<SubCategory>? ListSubCategory { get; set; }

        public string rootPathUpload = "";

        public EditCategoryModel(IConfiguration config, IMapper mapper, ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _config = config;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;

#if DEBUG
            //rootPathUpload = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/wwwroot"; // "Sophie/bin/Debug/net5.0/wwwroot"
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
#else
                //rootPathUpload = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/wwwroot"; // "Sophie/bin/Debug/net5.0/wwwroot"
                rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
#endif
        }

        public void OnGet()
        {
            SubCategory = new SubCategory()
            {
                SubCategoryId = Guid.NewGuid().ToString(),
                SubCategoryName = "",
                SubCategoryLevel = "2",
                SubCategoryIcon = "/cart/category.png",
                Type = TypeSubCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
        }

        public IActionResult OnGetFind(string categoryId, string subCategoryId)
        {


            Category = _categoryRepository.FindByIdCategory(categoryId);
            if (Category == null)
            {
                StatusMessage = "Error: Category not found";
                return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
            }
            else
            {
                if (!string.IsNullOrEmpty(subCategoryId))
                    SubCategory = Category.ListSubCategory.Find(item => item.SubCategoryId == subCategoryId);
                if (SubCategory == null)
                    SubCategory = new SubCategory()
                    {
                        SubCategoryName = "",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    };

            }
            return Page();
        }

        public async Task<IActionResult> OnPostCreate(string categoryId, SubCategory model, IFormFile[] attachment)
        {
            foreach (var file in attachment)
            {
                if (file.Length / 1024 / 1024 >= 32)
                {
                    StatusMessage = "File more than 32MB";
                    return Page();
                }
            }
            Category = _categoryRepository.FindByIdCategory(categoryId);
            if (Category == null)
            {
                StatusMessage = "Error: Category not found";
                return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
            }

            if (string.IsNullOrEmpty(model.SubCategoryIcon) && attachment.Length == 0)
            {
                StatusMessage = "Error: SubCategoryIcon is empty";
                return Page();
            }

            if (string.IsNullOrEmpty(model.SubCategoryName))
            {
                StatusMessage = "Error SubCategoryName is empty";
                return Page();
            }

            if (Category.ListSubCategory == null) Category.ListSubCategory = new List<SubCategory>();

            int indexCreated = -1;
            for (int i = 0; i < Category.ListSubCategory.Count; i++)
            {
                if (Category.ListSubCategory[i].SubCategoryId == model.SubCategoryId)
                {
                    indexCreated = i;
                    break;
                }
            }

            if (indexCreated == -1)
            {
                Logs.debug($"Create sub category...");
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.SubCategoryIcon) && model.SubCategoryIcon.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.SubCategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{model.SubCategoryIcon}");
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
                    model.SubCategoryIcon = images[0];
                }
                else // copy from image default
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}.png";
                    System.IO.File.Copy($"{rootPathUpload}/cart/category.png", $"{uploadPath}/{fileName}");
                    model.SubCategoryIcon = $"{uploadDirecotroy}/{fileName}";
                }
                Category.ListSubCategory.Add(model);

                _categoryRepository.UpdateCategory(Category);
                StatusMessage = "Sub category created successfully";
            }
            else
            {
                Logs.debug($"Update sub category...");
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.SubCategoryIcon) && model.SubCategoryIcon.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.SubCategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{model.SubCategoryIcon}");
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
                    model.SubCategoryIcon = images[0];
                }
                Category.ListSubCategory[indexCreated] = model;
                _categoryRepository.UpdateCategory(Category);
                StatusMessage = "Sub category edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/Category/EditCategory?handler=Find&categoryId=" + categoryId);
        }

        public Task<JsonResult> OnGetList(string categoryId)
        {
            //foreach (var item in Request.Query.ToArray())
            //{
            //    Logs.debug("OnGetList query: " + item.Key + " = " + item.Value);
            //}
            int draw = int.Parse(Request.Query["draw"]);
            int start = int.Parse(Request.Query["start"]);
            int length = int.Parse(Request.Query["length"]);
            string search = Request.Query["search[value]"];
            string sortName = Request.Query["order[0][column]"];
            string sort = Request.Query["order[0][dir]"];

            long total = 0;
            int page = 0;

            List<SubCategory> listData = new List<SubCategory>();
            Category = _categoryRepository.FindByIdCategory(categoryId);
            if (Category == null)
            {
                total = 0;
                page = start / length;
                Logs.debug("Total: " + total);
                Logs.debug("Start: " + start);
                Logs.debug("Length: " + length);
                Logs.debug("Page: " + page);
                return Task.FromResult(new JsonResult(new { draw = draw, recordsTotal = total, recordsFiltered = total, data = listData }));
            }

            if (!string.IsNullOrEmpty(search))
            {
                List<SubCategory> listSearch = Category?.ListSubCategory?.FindAll(item => item.SubCategoryName.ToLower().IndexOf(search.ToLower()) >= 0);
                total = listSearch.Count;
                page = start / length;
                listData = listSearch.Skip(page * length).Take(length).ToList();
            }
            else
            {
                total = Category?.ListSubCategory?.Count ?? 0;
                page = start / length;
                listData = Category?.ListSubCategory?.Skip(page * length)?.Take(length).ToList();
            }

            Logs.debug("Total: " + total);
            Logs.debug("Start: " + start);
            Logs.debug("Length: " + length);
            Logs.debug("Page: " + page);
            return Task.FromResult(new JsonResult(new { draw = draw, recordsTotal = total, recordsFiltered = total, data = listData != null ? listData : new List<SubCategory>() }));
        }

        public IActionResult OnGetDelete(string categoryId, string subCategoryId)
        {
            Category = _categoryRepository.FindByIdCategory(categoryId);
            if (Category == null)
            {
                StatusMessage = "Error: Category not found";
                return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
            }
            else
            {
                List<SubCategory> _listSubCategories = new List<SubCategory>();
                foreach (var item in Category.ListSubCategory)
                {
                    if (item.SubCategoryId == subCategoryId)
                    {
                        // remove old file if upload
                        if (!string.IsNullOrEmpty(item.SubCategoryIcon) && item.SubCategoryIcon.IndexOf("uploads") >= 0)
                        {
                            if (System.IO.File.Exists($"{rootPathUpload}/{item.SubCategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{item.SubCategoryIcon}");
                        }
                    }
                    else
                    {
                        _listSubCategories.Add(item);
                    }
                }
                Category.ListSubCategory = _listSubCategories;
                _categoryRepository.UpdateCategory(Category);
            }

            StatusMessage = "Category deleted successfully";
            return LocalRedirect($"~/Admin/ShopPage/Category/EditCategory?handler=Find&categoryId=" + categoryId);
        }
    }
}

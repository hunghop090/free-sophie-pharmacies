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
    public class CreateCategoryModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        [TempData]
        public string StatusMessage { get; set; }

        //[BindProperty]
        //public List<Category>? ListCategorys { get; set; }

        [BindProperty]
        public Category? Category { get; set; }

        public string rootPathUpload = "";

        public CreateCategoryModel(IConfiguration config, IMapper mapper, ICategoryRepository categoryRepository, IProductRepository productRepository)
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
            Category = new Category()
            {
                CategoryName = "",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category.png",
                ListSubCategory = new List<SubCategory>() { },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now()
            };
            //ListCategorys = _categoryRepository.ListCategory(0, int.MaxValue);
        }

        public Task<JsonResult> OnGetList()
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

            Logs.debug("Total: " + total);
            Logs.debug("Start: " + start);
            Logs.debug("Length: " + length);
            Logs.debug("Page: " + page);
            return Task.FromResult(new JsonResult(new { draw = draw, recordsTotal = total, recordsFiltered = total, data = listData }));
        }

        public async Task<IActionResult> OnPostCreate(Category model, IFormFile[] attachment)
        {
            foreach (var file in attachment)
            {
                if (file.Length / 1024 / 1024 >= 32)
                {
                    StatusMessage = "File more than 32MB";
                    return Page();
                }
            }
            if (string.IsNullOrEmpty(model.CategoryName))
            {
                StatusMessage = "Error invalid input data";
                return Page();
            }
            if (string.IsNullOrEmpty(model.CategoryIcon) && attachment.Length == 0)
            {
                StatusMessage = "Error: CategoryIcon is empty";
                return Page();
            }
            Category? item = _categoryRepository.FindByIdCategory(model.CategoryId);
            if (item == null)
            {
                Logs.debug($"Create category...");
                Logs.debug($"File size: {attachment.Length}");
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.CategoryIcon) && model.CategoryIcon.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.CategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{model.CategoryIcon}");
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
                    model.CategoryIcon = images[0];
                }
                else // copy from image default
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}.png";
                    System.IO.File.Copy($"{rootPathUpload}/cart/category.png", $"{uploadPath}/{fileName}");
                    model.CategoryIcon = $"{uploadDirecotroy}/{fileName}";
                }

                _categoryRepository.CreateCategory(model);
                StatusMessage = "Category created successfully";
            }
            else
            {
                Logs.debug($"Update category...");
                Logs.debug($"File size: {attachment.Length}");
                if (attachment.Length > 0)
                {
                    string uploadDirecotroy = "/uploads/category";
                    string uploadPath = $"{rootPathUpload}/{uploadDirecotroy}";
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    // remove old file if upload
                    if (!string.IsNullOrEmpty(model.CategoryIcon) && model.CategoryIcon.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{model.CategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{model.CategoryIcon}");
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
                    model.CategoryIcon = images[0];
                }
                model.ListSubCategory = item.ListSubCategory;
                _categoryRepository.UpdateCategory(model);
                StatusMessage = "Category edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
        }

        public IActionResult OnGetDelete(string categoryId)
        {
            Category? item = _categoryRepository.FindByIdCategory(categoryId);
            if (item == null)
            {
                StatusMessage = "Error: Category not found";
            }
            else
            {
                // remove old file if upload
                if (!string.IsNullOrEmpty(item.CategoryIcon) && item.CategoryIcon.IndexOf("uploads") >= 0)
                {
                    if (System.IO.File.Exists($"{rootPathUpload}/{item.CategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{item.CategoryIcon}");
                }
                _categoryRepository.DeleteCategory(item.CategoryId);

                StatusMessage = "Category deleted successfully";
                return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
            }

            return Page();
        }

        public IActionResult OnPostFakeData()
        {
            // Category 1
            var category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = "Thuốc Tân dược - Không kê đơn",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_2.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc kháng dị ứng",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc Kháng viêm",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                     new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc ngừa thai",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                           new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc đường hô hấp",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc da liễu",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc giảm cân",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc mắt - tai - mũi - họng",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc tiêu hóa",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc dành cho nam",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc giảm đau hạ sốt",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc dành cho phụ nữ",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc thần kinh",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc cơ xương khớp",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Vitamin và khoáng chất",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Dầu gió, cù là",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thuốc khác",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            // Category 1
            category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                PharmacistId = Guid.NewGuid().ToString(),
                CategoryName = "Thuốc Tân dược - Kê đơn",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_1.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC GÂY TÊ, GÂY MÊ, THUỐC GIÃN CƠ, GIẢI GIÃN CƠ",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC GIẢM ĐAU, HẠ SỐT; CHỐNG VIÊM KHÔNG STEROID; THUỐC ĐIỀU TRỊ GÚT VÀ CÁC BỆNH XƯƠNG KHỚP",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC CHỐNG DỊ ỨNG VÀ DÙNG TRONG CÁC TRƯỜNG HỢP QUÁ MẪN",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC GIẢI ĐỘC VÀ CÁC THUỐC DÙNG TRONG TRƯỜNG HỢP NGỘ ĐỘC",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC CHỐNG CO GIẬT, CHỐNG ĐỘNG KINH",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ KÝ SINH TRÙNG, CHỐNG NHIỄM KHUẨN",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ ĐAU NỬA ĐẦU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ UNG THƯ VÀ ĐIỀU HÒA MIỄN DỊCH",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ BỆNH ĐƯỜNG TIẾT NIỆU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC CHỐNG PARKINSON",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC TÁC DỤNG ĐỐI VỚI MÁU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                      new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC TIM MẠCH",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                           new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ BỆNH DA LIỄU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC TẨY TRÙNG VÀ SÁT KHUẨN",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                                              new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC LỢI TIỂU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐƯỜNG TIÊU HÓA",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "HOCMON VÀ CÁC THUỐC TÁC ĐỘNG VÀO HỆ THỐNG NỘI TIẾT",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "HUYẾT THANH VÀ GLOBULIN MIỄN DỊCH",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC LÀM MỀM CƠ VÀ ỨC CHẾ CHOLINESTERASE",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC ĐIỀU TRỊ BỆNH MẮT, TAI MŨI HỌNG",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC CÓ TÁC DỤNG THÚC ĐẺ, CẦM MÁU SAU ĐẺ VÀ CHỐNG ĐẺ NON",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "DUNG DỊCH LỌC MÀNG BỤNG, LỌC MÁU",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC CHỐNG RỐI LOẠN TÂM THẦN VÀ THUỐC TÁC ĐỘNG LÊN HỆ THẦN KINH",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "THUỐC TÁC DỤNG TRÊN ĐƯỜNG HÔ HẤP",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "DUNG DỊCH ĐIỀU CHỈNH NƯỚC, ĐIỆN GIẢI, CÂN BẰNG ACID-BASE VÀ CÁC DUNG DỊCH TIÊM TRUYỀN KHÁC",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "KHOÁNG CHẤT VÀ VITAMIN",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            // Category 3
            category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = "Dược & Mỹ phẩm",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_5.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Chăm sóc toàn thân",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Chăm sóc da",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Chăm sóc tóc - da đầu",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Chăm sóc vùng mắt",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Mỹ phẩm trang điểm",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            // Category 4
            category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = "Vật tư y tế",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_6.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Dụng cụ y tế",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Dụng cụ theo dõi",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Dụng cụ sơ cứu",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Vật tư y tế",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            // Category 5
            category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = "Thực phẩm chức năng",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_4.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Sinh lý - nội tiết tố",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Sức khỏe - tim mạch",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                     new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Hỗ trợ tiêu hóa",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Thần kinh - bổ não",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Hỗ trợ làm đẹp",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Hỗ trợ điều trị",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                     new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Vitamin - Khoáng chất",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Dinh dưỡng",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Nguồn gốc thảo dược",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            // Category 6
            category = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = "Thuốc đông y",
                CategoryLevel = "1",
                CategoryIcon = "/cart/category_3.png",
                ListSubCategory = new List<SubCategory>()
                {
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Chế phẩm y học cổ truyền",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                    new SubCategory()
                    {
                        SubCategoryId = Guid.NewGuid().ToString(),
                        SubCategoryName = "Vị thuốc y học cổ truyền",
                        SubCategoryLevel = "2",
                        SubCategoryIcon = "/cart/category.png",
                        Type = TypeSubCategory.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    },
                },
                Type = TypeCategory.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            _categoryRepository.CreateCategory(category);

            StatusMessage = "Category fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
        }

        public IActionResult OnPostDeleteFakeData()
        {
            List<Category> listCategory = _categoryRepository.ListCategory(0, int.MaxValue);
            foreach (var item in listCategory)
            {
                // remove old file if upload
                if (!string.IsNullOrEmpty(item.CategoryIcon) && item.CategoryIcon.IndexOf("uploads") >= 0)
                {
                    if (System.IO.File.Exists($"{rootPathUpload}/{item.CategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{item.CategoryIcon}");
                }

                if (Category.ListSubCategory == null) Category.ListSubCategory = new List<SubCategory>();

                foreach (var sub in Category.ListSubCategory)
                {
                    // remove old file if upload
                    if (!string.IsNullOrEmpty(sub.SubCategoryIcon) && sub.SubCategoryIcon.IndexOf("uploads") >= 0)
                    {
                        if (System.IO.File.Exists($"{rootPathUpload}/{sub.SubCategoryIcon}")) System.IO.File.Delete($"{rootPathUpload}/{sub.SubCategoryIcon}");
                    }
                }

                _categoryRepository.DeleteCategory(item.CategoryId);
            }

            StatusMessage = "Category delete fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Category/CreateCategory");
        }

        public IActionResult OnGetFind(string categoryId)
        {
            Category = _categoryRepository.FindByIdCategory(categoryId);
            return Page();
        }

    }
}

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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Sophie.Controllers.API;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class CreateProductModel : PageModel
    {
        private readonly IConfiguration _config;

        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IShopRepository _shopRepository;
        private readonly ICategoryRepository _categoryRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Product? Product { get; set; }

        [BindProperty]
        public List<SelectListItem> ListSelectCategory { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem { Text = "Chọn category", Value = "" }
        };

        [BindProperty]
        public List<Category> ListCategory { get; set; }

        [BindProperty]
        public List<SubCategory> ListSubCategory { get; set; }

        [BindProperty]
        public List<ImageInfo> ListImageInfo { get; set; } = new List<ImageInfo>();

        [BindProperty]
        public Shop? Shop { get; set; }

        [BindProperty]
        public List<Product> ListProducts { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Shop> ListShop { get; set; }

        public string rootPathUpload = "";

        public CreateProductModel(IConfiguration config, IPharmacistRepository pharmacistRepository, IProductRepository productRepository, IShopRepository shopRepository, ICategoryRepository categoryRepository)
        {
            _config = config;
            _pharmacistRepository = pharmacistRepository;
            _productRepository = productRepository;
            _shopRepository = shopRepository;
            _categoryRepository = categoryRepository;
        }

        public void OnGet(string productId)
        {
            ListCategory = _categoryRepository.ListCategoryActive();
            ListSelectCategory = ListCategory.Select(x => new SelectListItem
            {
                Value = x.CategoryId,
                Text = x.CategoryName
            }).ToList();

            if (!string.IsNullOrEmpty(productId))
            {
                Product = _productRepository.FindByIdProduct(productId);
            }
            if (Product == null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Product = new Product()
                {
                    PharmacistId = userId,
                    Type = TypeEnum.Actived,
                };
            }
            if (Product.ProductImages != null)
            {
                foreach (var image in Product.ProductImages)
                {
                    AmazonUploader AU = new AmazonUploader(_config);

                    ListImageInfo.Add(new ImageInfo
                    {
                        KeyImage = image,
                        Url = AU.GetUrlImage(image, "/products/" + Product.ProductId)
                    });
                }
            }
            if (Product.ProductInfo == null)
            {
                Product.ProductInfo = new List<ProductInfo>();
            }
            ListShop = _shopRepository.FindAll();
            if (ListShop == null)
            {
                StatusMessage = "Error: Shop not found";
            }
        }

        public IActionResult OnPostCreate(Product model, IFormFile[] attachment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _shopRepository.FindByIdShop(model.ShopId);
            if (shop == null)
            {
                StatusMessage = "Shop is not exists";
                return Page();
            }
            foreach (var file in attachment)
            {
                if (file.Length / 1024 / 1024 >= 32)
                {
                    StatusMessage = "File more than 32MB";
                    return Page();
                }
            }
            AmazonUploader AU = new AmazonUploader(_config);
            if (string.IsNullOrEmpty(model.ProductName))
            {
                StatusMessage = "Error invalid input data";
                return Page();
            }
            List<ProductInfo> newListContent = new List<ProductInfo>();
            model.ProductInfo.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Content))
                    newListContent.Add(x);
            });
            model.ProductInfo = newListContent;
            if (string.IsNullOrEmpty(model.ProductId))
            {
                model.ProductId = Guid.NewGuid().ToString();
                if (attachment.Length > 0)
                {

                    foreach (var item in attachment)
                    {
                        var keyFile = AU.SendMyFileToS3(item, item.FileName, "products/" + model.ProductId).Result;
                        if (!string.IsNullOrEmpty(keyFile))
                            model.ProductImages.Add(keyFile);
                    }
                }
                model.PharmacistId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _productRepository.CreateProduct(model);
                StatusMessage = "Product created successfully";
            }
            else
            {
                var oldProduct = _productRepository.FindByIdProduct(model.ProductId);
                if (oldProduct == null)
                {
                    StatusMessage = "Product không tồn tại";
                    return Page();
                };
                model.Id = oldProduct.Id;
                List<string> deleteImages = new List<string>();
                foreach (var image in oldProduct.ProductImages)
                {
                    if (model.ProductImages != null && !model.ProductImages.Contains(image) && !string.IsNullOrEmpty(image))
                        deleteImages.Add(image);
                }
                if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);
                if (attachment.Length > 0)
                {
                    foreach (var item in attachment)
                    {

                        var keyFile = AU.SendMyFileToS3(item, item.FileName, "products/" + model.ProductId).Result;
                        if (!string.IsNullOrEmpty(keyFile))
                            model.ProductImages.Add(keyFile);
                    }
                }
                _productRepository.UpdateProduct(model);
                StatusMessage = "Product edited successfully";
            }

            return LocalRedirect($"~/Admin/ShopPage/Product/ListProduct");
        }

        public IActionResult OnGetDelete(string productId)
        {
            AmazonUploader AU = new AmazonUploader(_config);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _shopRepository.FindByIdPharmacist(userId);
            Product? item = _productRepository.FindByIdProduct(productId);
            if (item == null || item.ShopId != shop.ShopId)
            {
                StatusMessage = "Error: Product not found";
            }
            else
            {
                _productRepository.DeleteProduct(item.ProductId);
                List<string> deleteImages = new List<string>();
                foreach (var image in item.ProductImages)
                {
                    if (!string.IsNullOrEmpty(image))
                        deleteImages.Add(image);
                }
                if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);

                StatusMessage = "Product deleted successfully";
                return LocalRedirect($"~/Admin/ShopPage/Product/ListProduct");
            }
            return Page();
        }

        public IActionResult OnPostFakeData()
        {
            var EmailAdmin = _config["EmailAdmin"];
            Pharmacist pharmacist = _pharmacistRepository.FindByEmailPharmacist(EmailAdmin);
            if (pharmacist == null)
            {
                StatusMessage = "Pharmacis admin not exits";
                return Page();
            }
            var pharmacistId = pharmacist.PharmacistId;
            List<Product> products = new List<Product>();
            var shop = _shopRepository.FindAll().Where(x => x.Type == TypeShop.Actived).FirstOrDefault();
            if (shop == null)
            {
                StatusMessage = "shop is not exits";
                return LocalRedirect($"~/Admin/ShopPage/Product/CreateProduct");
            }
            ListCategory = _categoryRepository.ListCategoryActive();
            if (ListCategory.Count < 2)
            {
                StatusMessage = "Error: Category not exist";
                return LocalRedirect($"~/Admin/ShopPage/Product/CreateProduct");
            }
            // product 1 - ListCategory[0]
            var product = new Product()
            {
                CategoryId = ListCategory[0].CategoryId,
                ShopId = shop.ShopId,
                PharmacistId = pharmacistId,
                ProductName = "Thuốc Acuvail 4,5mg/ml (30 ống x 0.4ml/hộp)",
                ProductImages = new List<string>() { "sample/products/product_1.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 80000,
                ProductRealPrice = 100000,
                ProductDiscounts = 20,
                ProductNumber = 1000,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"Có thể dùng trước hoặc sau bữa ăn.
Viên bao đường – bao phim: Người lớn: 2 – 4 viên/lần x 3 lần/ngày. Trẻ em: 1 – 2 viên/lần x 3 lần/ngày.
Viên nang mềm (hàm lượng gấp đôi viên nén): Người lớn: 1 – 2 viên/lần x 3 lần/ngày. Trẻ em trên 8 tuổi: 1viên/lần x 3 lần/ ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Chỉ định",
                        Content = @"Bổ gan, dùng phòng và hỗ trợ điều trị suy giảm chức năng gan, đặc biệt do dùng nhiều bia, rượu; viêm gan do thuốc, hóa chất
Làm giảm các triệu chứng của bệnh viêm gan gây mệt mỏi, vàng da, ăn kém, khó tiêu, bí đại tiểu tiện, táo bón.
Điều trị dị ứng, mụn nhọt, lở ngứa, nổi mề đay do bệnh gan gây ra.
Phòng và hỗ trợ điều trị vữa xơ động mạch, mỡ trong máu cao.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "Thích hợp dùng cho:",
                        Content = @"Người uống nhiều bia rượu, dùng nhiều thuốc gây hại cho gan.
Người trung và cao tuổi bị suy giảm chức năng gan: mệt mỏi, rối loạn tiêu hóa, táo bón.
Người bị mụn nhọt, trứng cá, dị ứng do gan nóng, chức năng thải độc của gan kém",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product);
            // product 2 - ListCategory[0]
            var product2 = new Product()
            {
                CategoryId = ListCategory[0].CategoryId,
                PharmacistId = pharmacistId,
                ShopId = shop.ShopId,
                ProductName = "Alphagan P (Hộp 1 chai 5ml)",
                ProductImages = new List<string>() { "sample/products/product_2.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 160000,
                ProductRealPrice = 200000,
                ProductDiscounts = 20,
                ProductNumber = 1000,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"* Người lớn: uống 2 – 3 viên/lần x 2 – 3 lần/ngày.
* Trẻ em: uống 1 viên/lần x 2 – 3 lần/ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Trình bày:",
                        Content = @"Hộp 1, 2, 5 vỉ x 20 viên bao phim.
Hộp 1, 2, 5 vỉ x 20 viên bao đường.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "HOẠT HUYẾT DƯỠNG NÃO",
                        Content = @"thích hợp với người hoạt động trí óc căng thẳng bị suy giảm trí nhớ, kém tập trung, mệt mỏi.",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product2);
            // product 3 - ListCategory[0]
            var product3 = new Product()
            {
                CategoryId = ListCategory[0].CategoryId,
                ShopId = shop.ShopId,
                PharmacistId = pharmacistId,
                ProductName = "Nước tẩy trang cho da nhạy cảm H2O Bioderma Sensibio",
                ProductImages = new List<string>() { "sample/products/product_3.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 240000,
                ProductRealPrice = 300000,
                ProductDiscounts = 20,
                ProductNumber = 100,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"Có thể dùng trước hoặc sau bữa ăn.
Viên bao đường – bao phim: Người lớn: 2 – 4 viên/lần x 3 lần/ngày. Trẻ em: 1 – 2 viên/lần x 3 lần/ngày.
Viên nang mềm (hàm lượng gấp đôi viên nén): Người lớn: 1 – 2 viên/lần x 3 lần/ngày. Trẻ em trên 8 tuổi: 1viên/lần x 3 lần/ ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Chỉ định",
                        Content = @"Bổ gan, dùng phòng và hỗ trợ điều trị suy giảm chức năng gan, đặc biệt do dùng nhiều bia, rượu; viêm gan do thuốc, hóa chất
Làm giảm các triệu chứng của bệnh viêm gan gây mệt mỏi, vàng da, ăn kém, khó tiêu, bí đại tiểu tiện, táo bón.
Điều trị dị ứng, mụn nhọt, lở ngứa, nổi mề đay do bệnh gan gây ra.
Phòng và hỗ trợ điều trị vữa xơ động mạch, mỡ trong máu cao.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "Thích hợp dùng cho:",
                        Content = @"Người uống nhiều bia rượu, dùng nhiều thuốc gây hại cho gan.
Người trung và cao tuổi bị suy giảm chức năng gan: mệt mỏi, rối loạn tiêu hóa, táo bón.
Người bị mụn nhọt, trứng cá, dị ứng do gan nóng, chức năng thải độc của gan kém",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product3);
            // product 4 - ListCategory[0]
            var product4 = new Product()
            {
                CategoryId = ListCategory[0].CategoryId,
                PharmacistId = pharmacistId,
                ShopId = shop.ShopId,
                ProductName = "Gel tắm làm dịu bảo vệ da khô, nhạy cảm Bioderma Douche ",
                ProductImages = new List<string>() { "sample/products/product_4.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 320000,
                ProductRealPrice = 400000,
                ProductDiscounts = 20,
                ProductNumber = 100,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"* Người lớn: uống 2 – 3 viên/lần x 2 – 3 lần/ngày.
* Trẻ em: uống 1 viên/lần x 2 – 3 lần/ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Trình bày:",
                        Content = @"Hộp 1, 2, 5 vỉ x 20 viên bao phim.
Hộp 1, 2, 5 vỉ x 20 viên bao đường.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "HOẠT HUYẾT DƯỠNG NÃO",
                        Content = @"thích hợp với người hoạt động trí óc căng thẳng bị suy giảm trí nhớ, kém tập trung, mệt mỏi.",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product4);
            // product 5 - ListCategory[1]
            var product5 = new Product()
            {
                CategoryId = ListCategory[1].CategoryId,
                ShopId = shop.ShopId,
                PharmacistId = pharmacistId,
                ProductName = "Dầu xả ngăn gãy rụng tóc Dove (320g)",
                ProductImages = new List<string>() { "sample/products/product_5.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 80000,
                ProductRealPrice = 100000,
                ProductDiscounts = 20,
                ProductNumber = 1000,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"Có thể dùng trước hoặc sau bữa ăn.
Viên bao đường – bao phim: Người lớn: 2 – 4 viên/lần x 3 lần/ngày. Trẻ em: 1 – 2 viên/lần x 3 lần/ngày.
Viên nang mềm (hàm lượng gấp đôi viên nén): Người lớn: 1 – 2 viên/lần x 3 lần/ngày. Trẻ em trên 8 tuổi: 1viên/lần x 3 lần/ ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Chỉ định",
                        Content = @"Bổ gan, dùng phòng và hỗ trợ điều trị suy giảm chức năng gan, đặc biệt do dùng nhiều bia, rượu; viêm gan do thuốc, hóa chất
Làm giảm các triệu chứng của bệnh viêm gan gây mệt mỏi, vàng da, ăn kém, khó tiêu, bí đại tiểu tiện, táo bón.
Điều trị dị ứng, mụn nhọt, lở ngứa, nổi mề đay do bệnh gan gây ra.
Phòng và hỗ trợ điều trị vữa xơ động mạch, mỡ trong máu cao.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "Thích hợp dùng cho:",
                        Content = @"Người uống nhiều bia rượu, dùng nhiều thuốc gây hại cho gan.
Người trung và cao tuổi bị suy giảm chức năng gan: mệt mỏi, rối loạn tiêu hóa, táo bón.
Người bị mụn nhọt, trứng cá, dị ứng do gan nóng, chức năng thải độc của gan kém",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product5);
            // product 6 - ListCategory[1]
            var product6 = new Product()
            {
                CategoryId = ListCategory[1].CategoryId,
                PharmacistId = pharmacistId,
                ShopId = shop.ShopId,
                ProductName = "Sữa tắm dưỡng thể Dove Deeply Nourishing (530g)",
                ProductImages = new List<string>() { "sample/products/product_6.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 160000,
                ProductRealPrice = 200000,
                ProductDiscounts = 20,
                ProductNumber = 1000,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"* Người lớn: uống 2 – 3 viên/lần x 2 – 3 lần/ngày.
* Trẻ em: uống 1 viên/lần x 2 – 3 lần/ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Trình bày:",
                        Content = @"Hộp 1, 2, 5 vỉ x 20 viên bao phim.
Hộp 1, 2, 5 vỉ x 20 viên bao đường.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "HOẠT HUYẾT DƯỠNG NÃO",
                        Content = @"thích hợp với người hoạt động trí óc căng thẳng bị suy giảm trí nhớ, kém tập trung, mệt mỏi.",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product6);
            // product 7 - ListCategory[1]
            var product7 = new Product()
            {
                CategoryId = ListCategory[1].CategoryId,
                ShopId = shop.ShopId,
                PharmacistId = pharmacistId,
                ProductName = "Thuốc Acuvail 4,5mg/ml (30 ống x 0.4ml/hộp)",
                ProductImages = new List<string>() { "sample/products/product_7.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 240000,
                ProductRealPrice = 300000,
                ProductDiscounts = 20,
                ProductNumber = 100,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"Có thể dùng trước hoặc sau bữa ăn.
Viên bao đường – bao phim: Người lớn: 2 – 4 viên/lần x 3 lần/ngày. Trẻ em: 1 – 2 viên/lần x 3 lần/ngày.
Viên nang mềm (hàm lượng gấp đôi viên nén): Người lớn: 1 – 2 viên/lần x 3 lần/ngày. Trẻ em trên 8 tuổi: 1viên/lần x 3 lần/ ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Chỉ định",
                        Content = @"Bổ gan, dùng phòng và hỗ trợ điều trị suy giảm chức năng gan, đặc biệt do dùng nhiều bia, rượu; viêm gan do thuốc, hóa chất
Làm giảm các triệu chứng của bệnh viêm gan gây mệt mỏi, vàng da, ăn kém, khó tiêu, bí đại tiểu tiện, táo bón.
Điều trị dị ứng, mụn nhọt, lở ngứa, nổi mề đay do bệnh gan gây ra.
Phòng và hỗ trợ điều trị vữa xơ động mạch, mỡ trong máu cao.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "Thích hợp dùng cho:",
                        Content = @"Người uống nhiều bia rượu, dùng nhiều thuốc gây hại cho gan.
Người trung và cao tuổi bị suy giảm chức năng gan: mệt mỏi, rối loạn tiêu hóa, táo bón.
Người bị mụn nhọt, trứng cá, dị ứng do gan nóng, chức năng thải độc của gan kém",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product7);
            // product 8 - ListCategory[1]
            var product8 = new Product()
            {
                CategoryId = ListCategory[1].CategoryId,
                PharmacistId = pharmacistId,
                ShopId = shop.ShopId,
                ProductName = "Dầu cá Nature's Care Omega 3 Fish Oil 1000mg (Hộp 200v)",
                ProductImages = new List<string>() { "sample/products/product_8.png", "sample/products/temp_medicine_video.mp4" },
                ProductPrice = 320000,
                ProductRealPrice = 400000,
                ProductDiscounts = 20,
                ProductNumber = 100,
                PurchasedNumber = 0,
                Rating = 1.0,
                SellOver = true,
                ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"* Người lớn: uống 2 – 3 viên/lần x 2 – 3 lần/ngày.
* Trẻ em: uống 1 viên/lần x 2 – 3 lần/ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Trình bày:",
                        Content = @"Hộp 1, 2, 5 vỉ x 20 viên bao phim.
Hộp 1, 2, 5 vỉ x 20 viên bao đường.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "HOẠT HUYẾT DƯỠNG NÃO",
                        Content = @"thích hợp với người hoạt động trí óc căng thẳng bị suy giảm trí nhớ, kém tập trung, mệt mỏi.",
                    },
                },
                Type = TypeEnum.Actived,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now(),
            };
            products.Add(product8);

            // product 10 to 99
            for (int i=2; i < ListCategory.Count; i++)
            {
                for (var j = 0; j < 300; j++)
                {
                    var newProduct = new Product()
                    {
                        CategoryId = ListCategory[i].CategoryId,
                        PharmacistId = pharmacistId,
                        ShopId = shop.ShopId,
                        ProductName = $"{ListCategory[i].CategoryName} loại {j}",
                        ProductImages = new List<string>() { "sample/products/temp_medicine_image.jpeg", "sample/products/temp_medicine_video.mp4" },
                        ProductPrice = 160 * j,
                        ProductRealPrice = 20000 * j,
                        ProductNumber = 15 * j,
                        PurchasedNumber = 5 * j,
                        Rating = 1.0,
                        SellOver = true,
                        ProductInfo = new List<ProductInfo>()
                {
                    new ProductInfo()
                    {
                        Title = "Liều dùng – Cách dùng",
                        Content = @"* Người lớn: uống 2 – 3 viên/lần x 2 – 3 lần/ngày.
* Trẻ em: uống 1 viên/lần x 2 – 3 lần/ngày.",
                    },
                       new ProductInfo()
                    {
                        Title = "Trình bày:",
                        Content = @"Hộp 1, 2, 5 vỉ x 20 viên bao phim.
Hộp 1, 2, 5 vỉ x 20 viên bao đường.",
                    },
                            new ProductInfo()
                    {
                        Title = "Chống chỉ định",
                        Content = @"Người mẫn cảm với thành phần của thuốc",
                    },
                                 new ProductInfo()
                    {
                        Title = "HOẠT HUYẾT DƯỠNG NÃO",
                        Content = @"thích hợp với người hoạt động trí óc căng thẳng bị suy giảm trí nhớ, kém tập trung, mệt mỏi.",
                    },
                },
                        Type = TypeEnum.Actived,
                        Created = DateTimes.Now(),
                        Updated = DateTimes.Now(),
                    };
                    products.Add(newProduct);
                }
            }
            
            _productRepository.CreateProducts(products);

            StatusMessage = "Category fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Product/CreateProduct");
        }

        public IActionResult OnPostDeleteFakeData()
        {
            var EmailAdmin = _config["EmailAdmin"];
            Pharmacist pharmacist = _pharmacistRepository.FindByEmailPharmacist(EmailAdmin);
            if (pharmacist == null)
            {
                StatusMessage = "Pharmacis admin not exits";
                return Page();
            }
            var pharmacistId = pharmacist.PharmacistId;
            AmazonUploader AU = new AmazonUploader(_config);
            var paging = new Paging()
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
            };
            PagingResult<Product> listProduct = _productRepository.ListProductByPharmacistId(paging, pharmacistId);
            List<string> deleteImages = new List<string>();
            foreach (var item in listProduct.Result)
            {
                _productRepository.DeleteProduct(item.ProductId);
                foreach (var image in item.ProductImages)
                {
                    if (!string.IsNullOrEmpty(image) && !deleteImages.Contains(image))
                        deleteImages.Add(image);
                }
            }
            if (deleteImages.Count > 0) AU.MultiObjectDeleteAsync(deleteImages);

            StatusMessage = "Product delete fake data successfully";
            return LocalRedirect($"~/Admin/ShopPage/Product/CreateProduct");
        }

    }
}

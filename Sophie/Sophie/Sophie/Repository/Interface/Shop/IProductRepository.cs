using System;
using System.Collections.Generic;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using static Sophie.Controllers.API.ProductController;

namespace Sophie.Repository.Interface
{
    public interface IProductRepository
    {

        Product CreateProduct(Product item);
        Product DeleteProduct(string productId);
        Product FindByIdProduct(string productId);
        PagingResult<Product> ListProduct(Paging paging);
        PagingResult<Product> ListSearchProduct(Paging paging);
        PagingResult<Product> ListTopSold(Paging paging);
        PagingResult<Product> ListProductFilter(FilterProduct filter, SortType? sort, SortFilterProduct? sortName, int limit, int pageIndex);
        List<Product> FindAll();
        Product UpdateProduct(Product item);
        long TotalProduct();
        void CreateProducts(List<Product> products);
        List<Product> FindByProductIds(string productIds = "");
        void UpdatePurchasedNumber(List<ProductOrder> listProduct);
        PagingResult<Product> ListProductByPharmacistId(Paging paging, string pharmacistId);
    }
}

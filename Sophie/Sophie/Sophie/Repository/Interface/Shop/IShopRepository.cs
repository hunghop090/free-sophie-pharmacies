using System.Collections.Generic;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface IShopRepository
    {
        Shop CreateShop(Shop item);
        Shop DeleteShop(string shopId);
        Shop FindByIdShop(string shopId);
        List<Shop> FindByIdShops(string shopIds);
        Shop FindByIdPharmacist(string pharmacistId);
        PagingResult<Shop> ListShop(Paging paging);
        List<Shop> FindAll();
        Shop UpdateShop(Shop item);
        long TotalShop();
    }
}

using System.Collections.Generic;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface IPromotionRepository
    {
        Promotion CreatePromotion(Promotion item);
        Promotion DeletePromotion(string promotionId);
        Promotion FindByIdPromotion(string promotionId);
        PagingResult<Promotion> ListPromotion(Paging paging);
        PagingResult<Promotion> ListPromotionActive(Paging paging);
        PagingResult<Promotion> ListPromotionByShopIds(FilterWithId filter);
        List<Promotion> FindAll();
        Promotion UpdatePromotion(Promotion item);
        long TotalPromotion();
        Promotion FindByCode(string promotionCode);
        List<Promotion> FindByPromotionIds(TypePay? typePay, List<string> promotionIds);
        void UpdateUsed(List<string> listPromotion);
    }
}

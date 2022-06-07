using System.Collections.Generic;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface ITransportPromotionRepository
    {
        TransportPromotion CreateTransportPromotion(TransportPromotion item);
        TransportPromotion DeleteTransportPromotion(string transportPromotionId);
        TransportPromotion FindByIdTransportPromotion(string transportPromotionId);
        PagingResult<TransportPromotion> ListTransportPromotion(Paging paging);
        PagingResult<TransportPromotion> ListPromotionActive(Paging paging);
        PagingResult<TransportPromotion> ListTransportPromotionByShopIds(FilterWithId filter);
        List<TransportPromotion> FindAll();
        TransportPromotion UpdateTransportPromotion(TransportPromotion item);
        long TotalTransportPromotion();
        TransportPromotion FindByCode(string transportPromotionCode);
        List<TransportPromotion> FindByTransportPromotionIds(TypePay? typePay, List<string> transportPromotionIds);
        void UpdateUsed(List<string> listPromotion);
    }
}

using System.Collections.Generic;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface IDraftOrderRepository
    {
        DraftOrder CreateDraftOrder(DraftOrder item);
        DraftOrder DeleteDraftOrder(string draftOrderId);
        DraftOrder FindByIdDraftOrder(string draftOrderId);
        DraftOrder FindByAccountId(string accountId);
        PagingResult<DraftOrder> ListDraftOrder(Paging paging);
        List<DraftOrder> FindAll(string pharmacistId);
        DraftOrder UpdateDraftOrder(DraftOrder item);
        DraftOrder UpdateDraftOrderById(DraftOrder item);
        DraftOrder UpdateTransactionId(string draftOrderId, string transactionId);
        long TotalDraftOrder();
        DraftOrder FindByTransactionId(string transactionId);
    }
}

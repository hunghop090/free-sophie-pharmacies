using System.Collections.Generic;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Repository.Interface
{
    public interface IOrderRepository
    {
        Order CreateOrder(Order item);
        Order DeleteOrder(string orderId);
        Order FindByIdOrder(string orderId);
        PagingResult<OrderGroupBy> ListOrder(Paging paging);
        List<Order> FindAll(string pharmacistId);
        Order UpdateOrder(Order item);
        long TotalOrder();
        List<Order> FindByIdTransactionId(string transactionId);
        List<Order> CreateListOrder(List<Order> orders);
        Order FindExitsByShopId(string shopId);
        PagingResult<OrderGroupBy> FindByAccountId(FilterWithId filter);
        bool UpdateListOrder(List<Order> listOrder, TypeStatusOrder typeStatusOrder);

        bool DeleteSameTransactionId(string transactionId, string accountId);
    }
}

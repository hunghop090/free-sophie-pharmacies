using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Core.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Sophie.Repository.Interface;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class ListOrderModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly IOrderRepository _orderRepository;
        private readonly IShopRepository _shopRepository;

        [TempData]
        public string StatusMessage { get; set; }

        public ListOrderModel(IConfiguration config, IMapper mapper, IOrderRepository orderRepository, IShopRepository shopRepository)
        {
            _config = config;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _shopRepository = shopRepository;
        }

        public Task<JsonResult> OnGetList()
        {
            int draw = int.Parse(Request.Query["draw"]);
            int start = int.Parse(Request.Query["start"]);
            int length = int.Parse(Request.Query["length"]);
            string search = Request.Query["search[value]"];
            string sortName = Request.Query["order[0][column]"];
            string sort = Request.Query["order[0][dir]"];

            Paging paging = new Paging
            {
                PageIndex = start / length,
                PageSize = length,
                search = search,
                sortName = String.IsNullOrEmpty(sortName) ? "Created" : sortName,
                sort = String.IsNullOrEmpty(sort) ? "desc" : sort,
            };
            PagingResult<OrderGroupBy> listSearch = _orderRepository.ListOrder(paging);
            List<OrderGroupByDto> listOrderDto = _mapper.Map<List<OrderGroupByDto>>(listSearch.Result);
            List<String> idsShop = new List<String>();
            foreach (var data in listOrderDto)
            {
                data.Price = data.ListOrder.Sum(x => x.Price);
                data.TransportPromotionPrice = data.ListOrder.Sum(x => x.TransportPromotionPrice);
                data.TransportPrice = data.ListOrder.Sum(x => x.TransportPrice);
                data.PromotionPrice = data.ListOrder.Sum(x => x.PromotionPrice);
                data.AccountName = data.ListOrder.FirstOrDefault().AccountName;
                foreach (var order in data.ListOrder)
                {
                    if (!idsShop.Contains(order.ShopId))
                    {
                        idsShop.Add(order.ShopId);
                    }
                }
            }
            var ListShop = _shopRepository.FindByIdShops(String.Join(",", idsShop));
            foreach (var data in listOrderDto)
            {
                foreach (var order in data.ListOrder)
                {
                    order.ShopName = ListShop.Where(x => x.ShopId == order.ShopId).FirstOrDefault()?.ShopName;
                }
            }

            return Task.FromResult(new JsonResult(new
            {
                draw = draw,
                recordsTotal = listSearch.Total,
                recordsFiltered = listSearch.Total,
                data = listOrderDto
            }));
        }
    }
}

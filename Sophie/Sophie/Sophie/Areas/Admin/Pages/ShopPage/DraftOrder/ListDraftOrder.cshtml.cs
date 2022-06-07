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
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;

namespace Sophie.Areas.Admin.ShopPage
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class ListDraftOrderModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly IDraftOrderRepository _draftOrderRepository;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<DraftOrder>? ListDraftOrders { get; set; }

        public ListDraftOrderModel(IConfiguration config, IMapper mapper, IDraftOrderRepository draftOrderRepository)
        {
            _config = config;
            _mapper = mapper;
            _draftOrderRepository = draftOrderRepository;
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
                sortName = String.IsNullOrEmpty(sortName) ? "Updated" : sortName,
                sort = String.IsNullOrEmpty(sort) ? "desc" : sort,
            };
            PagingResult<DraftOrder> listSearch = _draftOrderRepository.ListDraftOrder(paging);
            ListDraftOrders = listSearch.Result;
            return Task.FromResult(new JsonResult(new { draw = draw, recordsTotal = listSearch.Total, recordsFiltered = listSearch.Total, data = listSearch.Result }));
        }
    }
}

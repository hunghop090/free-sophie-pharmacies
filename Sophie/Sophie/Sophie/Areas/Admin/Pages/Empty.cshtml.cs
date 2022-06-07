using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sophie.Areas.Admin.Pages
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class EmptyModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        public EmptyModel()
        {
        }

        public void OnGet()
        {
        }
    }
}

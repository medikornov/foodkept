using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FoodKept.Pages.FoodPages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private UserManager<IdentityUser> _userManager;

        public IndexModel(FoodKept.Data.ShopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Food> Food { get;set; }
        public string id { get; set; }

        public async Task OnGetAsync()
        {
            Food = await _context.FoodData.ToListAsync();

            id = _userManager.GetUserId(User);
        }
    }
}

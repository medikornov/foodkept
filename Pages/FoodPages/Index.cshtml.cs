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
using static System.Net.Mime.MediaTypeNames;
using FoodKept.ViewModels;
using FoodKept.Helpers;

namespace FoodKept.Pages.FoodPages
{
    [Authorize(Roles ="Admin, Restaurant")]
    public class IndexModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ModifiedList<Food> Food { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public async Task OnGetAsync()
        {
            //Query for food from current user
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            Food = new ModifiedList<Food>(_context.FoodData.Include(c => c.ApplicationUser).Where(c => c.ApplicationUserId == applicationUser.Id).ToList());
           
            //Calculate Discounts
            CalculateCurrentPrice.CalculatePriceForFoodList(Food);

            //Filter food
            var foods = from m in _context.FoodData
                        select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                foods = foods.Where(s => s.FoodName.Contains(SearchString) && s.ApplicationUserId == applicationUser.Id);
                Food = new ModifiedList<Food>(await foods.ToListAsync());
            }
        }
    }
}

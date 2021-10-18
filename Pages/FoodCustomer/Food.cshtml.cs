using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Helpers;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages
{
    [Authorize(Roles ="Customer, Admin")]
    public class FoodModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private UserManager<ApplicationUser> _userManager;

        public FoodModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Food> Food { get; set; }
        public string id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }


        public async Task OnGetAsync()
        {
            //Query food
            Food = _context.FoodData.ToList();

            //Calculate Discounts
            CalculateCurrentPrice.CalculatePriceForFoodList(Food);

            var foods = from m in _context.FoodData
                        select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                foods = foods.Where(s => s.FoodName.Contains(SearchString));
            }
            Food = await foods.ToListAsync();

            id = _userManager.GetUserId(User);
        }
    }
}

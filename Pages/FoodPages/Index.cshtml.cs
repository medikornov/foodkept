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

        public PaginatedList<Food> Food { get; set; }
        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string DiscountSort { get; set; }
        public string QuantitySort { get; set; }
        public string FoodCategorySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            //Sorting system
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            DiscountSort = sortOrder == "Discount" ? "discount_desc" : "Discount";
            QuantitySort = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            FoodCategorySort = string.IsNullOrEmpty(sortOrder) ? "category_desc" : "Category";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            //Query for food from current user
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            IQueryable<Food> foodIQ = _context.FoodData.Include(c => c.ApplicationUser).Where(c => c.ApplicationUserId == applicationUser.Id);

            //Filter food
            if (!string.IsNullOrEmpty(searchString))
            {
                foodIQ = foodIQ.Where(s => s.FoodName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.FoodName);
                    break;
                case "Price":
                    foodIQ = foodIQ.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.Price);
                    break;
                case "Discount":
                    foodIQ = foodIQ.OrderBy(s => s.CurrentPrice.DiscountPercent);
                    break;
                case "discount_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.CurrentPrice.DiscountPercent);
                    break;
                case "Quantity":
                    foodIQ = foodIQ.OrderBy(s => s.Quantity);
                    break;
                case "quantity_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.Quantity);
                    break;
                case "Category":
                    foodIQ = foodIQ.OrderBy(s => s.FoodCategory);
                    break;
                case "category_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.FoodCategory);
                    break;
                default:
                    foodIQ = foodIQ.OrderBy(s => s.FoodName);
                    break;
            }

            //Calculate Discounts
            CalculateCurrentPrice.CalculatePriceForFoodList(Food: await foodIQ.ToListAsync());

            Food = await PaginatedList<Food>.CreateAsync(foodIQ, pageIndex ?? 1, 5);
        }
    }
}

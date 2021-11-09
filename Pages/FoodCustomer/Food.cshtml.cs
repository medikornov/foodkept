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
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Food> Food { get; set; }
        public string id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public string NameSort { get; set; }
        public string RestaurantSort { get; set; }
        public string PriceSort { get; set; }
        public string DiscountSort { get; set; }
        public string QuantitySort { get; set; }
        public string FoodCategorySort { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            //Lazy Initialization
            //Lazy<List<Food>> getFood = new Lazy<List<Food>>(() => _context.FoodData.ToList());

            //Sorting system
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            RestaurantSort = string.IsNullOrEmpty(sortOrder) ? "resName_desc" : "Restaurant";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            DiscountSort = sortOrder == "Discount" ? "discount_desc" : "Discount";
            QuantitySort = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            FoodCategorySort = string.IsNullOrEmpty(sortOrder) ? "category_desc" : "Category";

            IQueryable<Food> foodIQ = from s in _context.FoodData
                                             select s;

            switch (sortOrder)
            {
                case "name_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.FoodName);
                    break;
                case "Restaurant":
                    foodIQ = foodIQ.OrderBy(s => s.ApplicationUser.RestaurantName);
                    break;
                case "resName_desc":
                    foodIQ = foodIQ.OrderByDescending(s => s.ApplicationUser.RestaurantName);
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

            Food = await foodIQ.ToListAsync();

            //Calculate Discounts
            CalculateCurrentPrice.CalculatePriceForFoodList(Food: Food);

            var foods = from m in _context.FoodData
                        select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                foods = foods.Where(s => s.FoodName.Contains(SearchString));
                Food = await foods.ToListAsync();
            }

            id = _userManager.GetUserId(User);
        }
    }
}

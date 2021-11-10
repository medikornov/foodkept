using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Helpers;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FoodKept.ViewModels;

namespace FoodKept.Pages
{
    [Authorize(Roles = "Customer, Admin")]
    public class FoodModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public FoodModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [BindProperty]
        public PaginatedList<Food> Food { get; set; }
        [BindProperty]
        public List<SelectListItem> FoodCategory { get; set; }
        public string id { get; set; }
        public string NameSort { get; set; }
        public string RestaurantSort { get; set; }
        public string PriceSort { get; set; }
        public string DiscountSort { get; set; }
        public string QuantitySort { get; set; }
        public string FoodCategorySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentCategory { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, string currentCategory, int? pageIndex)
        {
            //Lazy Initialization
            //Lazy<List<Food>> getFood = new Lazy<List<Food>>(() => _context.FoodData.ToList());

            //Load food categories
            FoodCategory = Enum.GetValues(typeof(FoodCategories.Category)).Cast<FoodCategories.Category>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = v.ToString()
            }).ToList();

            //Sorting system
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            RestaurantSort = string.IsNullOrEmpty(sortOrder) ? "resName_desc" : "Restaurant";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            DiscountSort = sortOrder == "Discount" ? "discount_desc" : "Discount";
            QuantitySort = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            FoodCategorySort = string.IsNullOrEmpty(sortOrder) ? "category_desc" : "Category";

            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            CurrentCategory = currentCategory;

            IQueryable<Food> foodIQ = from s in _context.FoodData
                                             select s;

            if(!string.IsNullOrEmpty(searchString))
            {
                foodIQ = foodIQ.Where(s => s.FoodName.Contains(searchString) || s.ApplicationUser.RestaurantName.Contains(searchString));
            }
            else if (!string.IsNullOrEmpty(CurrentCategory))
            {
                if(CurrentCategory != "None")
                    foodIQ = foodIQ.Where(s => s.FoodCategory.Contains(CurrentCategory));
            }

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

            //Calculate Discounts
            CalculateCurrentPrice.CalculatePriceForFoodList(Food: await foodIQ.ToListAsync());

            Food = await PaginatedList<Food>.CreateAsync(foodIQ, pageIndex ?? 1, 5);

            id = _userManager.GetUserId(User);
        }
    }
}

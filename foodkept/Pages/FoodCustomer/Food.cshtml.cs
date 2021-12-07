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
using System.Linq.Expressions;
using FoodKept.Data;

namespace FoodKept.Pages
{
    [Authorize(Roles = "Customer, Admin")]
    public class FoodModel : PageModel
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodModel(IFoodRepository foodRepository, UserManager<ApplicationUser> userManager, ShopContext context)
        {
            _foodRepository = foodRepository;
            _userManager = userManager;
            _context = context;
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
            //Load food categories
            FoodCategory = Enum.GetValues(typeof(FoodCategories.Category)).Cast<FoodCategories.Category>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = v.ToString()
            }).ToList();

            //Sorting system
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "_FoodName" : "FoodName";
            RestaurantSort = string.IsNullOrEmpty(sortOrder) ? "_RestaurantName" : "RestaurantName";
            PriceSort = sortOrder == "Price" ? "_Price" : "Price";
            DiscountSort = sortOrder == "Discount" ? "_Discount" : "Discount";
            QuantitySort = sortOrder == "Quantity" ? "_Quantity" : "Quantity";
            FoodCategorySort = string.IsNullOrEmpty(sortOrder) ? "_FoodCategory" : "FoodCategory";

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

            IQueryable<Food> foodIQ = _foodRepository.GetAllFood().AsQueryable().Include(s => s.Ratings);

            if (!string.IsNullOrEmpty(searchString))
            {
                foodIQ = foodIQ.Where(s => s.FoodName.Contains(searchString) || s.ApplicationUser.RestaurantName.Contains(searchString));
            }
            else if (!string.IsNullOrEmpty(CurrentCategory))
            {
                if(CurrentCategory != "None")
                    foodIQ = foodIQ.Where(s => s.FoodCategory.Contains(CurrentCategory));
            }

            //Sort everything
            Lazy<FoodSortHelper> foodSort = new Lazy<FoodSortHelper>(() => new FoodSortHelper());

            if(sortOrder != null && sortOrder[0] == '_')
            {
                foodIQ = foodSort.Value.SortCommandHandler[sortOrder](sortOrder.Substring(1), foodIQ);
            }
            else if(sortOrder != null)
            {
                foodIQ = foodSort.Value.SortCommandHandler[sortOrder](sortOrder, foodIQ);
            }

            Food = await PaginatedList<Food>.CreateAsync(foodIQ, pageIndex ?? 1, 5);

            //Calculate Discounts
            Food = await CalculateCurrentPrice.CalculatePriceForFoodListAsync(Food: Food);

            id = _userManager.GetUserId(User);
        }

        
        public async Task<IActionResult> OnPostRating(int id, int fid)
        {
            StarRating starRating = new StarRating();
            starRating.Rate = id;
            starRating.FoodId = fid;

            _context.StarRating.Add(starRating);
            await  _context.SaveChangesAsync();

            return new JsonResult("You rated this " + id.ToString() + "star(s)");
        }
    }
}

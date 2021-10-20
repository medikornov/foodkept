using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using FoodKept.Helpers;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FoodCustomer
{
    [Authorize(Roles = "Admin, Restaurant, Customer")]
    public class DetailsModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public DetailsModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        public Food Food { get; set; }
        public static Coordinates Coordinates { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == id);
            CalculateCurrentPrice.CalculatePriceForFood(Food);

            if (Food == null)
            {
                return NotFound();
            }

            //Configure Lat Lng
            if(Food.ApplicationUser.Lat != 0 && Food.ApplicationUser.Lng != 0)
            {
                Coordinates = new Coordinates(Food.ApplicationUser.Lat, Food.ApplicationUser.Lng);
            }
            else
            {
                Coordinates = new Coordinates(54.68587937724495, 25.278637513732985);
            }

            return Page();
        }
    }
}

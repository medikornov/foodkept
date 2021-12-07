using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;

namespace FoodKept.Pages.DiscountPages
{
    public class DetailsModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public DetailsModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        public Discount Discount { get; set; }
        [BindProperty]
        public Food Food { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? foodID)
        {
            if (id == null || foodID == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == foodID);
            Discount = await _context.DiscountData.FirstOrDefaultAsync(m => m.ID == id);

            if (Discount == null || Food == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

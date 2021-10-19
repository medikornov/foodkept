using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;

namespace FoodKept.Pages.DiscountPages
{
    public class EditModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public EditModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? foodID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == foodID);

            //Calculate Discount Percent
            CalculatePercentage(Food, Discount);
            _context.Attach(Discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(Discount.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            string url = "/DiscountPages?id=" + foodID;
            return Redirect(url);
        }

        private bool DiscountExists(int id)
        {
            return _context.DiscountData.Any(e => e.ID == id);
        }

        private static void CalculatePercentage(Food Food, Discount Discount)
        {
            int discountpart = (int)Math.Round((1 - (Discount.DiscountPrice / Food.Price)) * 100);
            Discount.DiscountPercent = discountpart;
        }
    }
}

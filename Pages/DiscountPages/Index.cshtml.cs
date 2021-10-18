using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;
using Newtonsoft.Json;

namespace FoodKept.Pages.DiscountPages
{
    public class IndexModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public IndexModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        public IList<Discount> Discount { get;set; }
        public Food Food { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == id);
            Discount = _context.DiscountData.Include(c => c.Food).Where(c => c.FoodId == Food.ID).ToList();

            if (Discount == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

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

namespace FoodKept.Pages.FoodPages
{
    [Authorize(Roles = "Admin, Restaurant")]
    public class DetailsModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public DetailsModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        public Food Food { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.FoodID == id);

            if (Food == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

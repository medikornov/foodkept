using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;

namespace FoodKept.Pages.FoodPages
{
    public class DeleteModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public DeleteModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Food Food { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == id);

            if (Food == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FindAsync(id);

            if (Food != null)
            {
                _context.FoodData.Remove(Food);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

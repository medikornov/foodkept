using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodKept.Pages.FoodPages
{
    public class CreateModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(FoodKept.Data.ShopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Food Food { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //If the user creates a food, that food is linked to the user by id
            Food.UserID = _userManager.GetUserId(User);

            _context.FoodData.Add(Food);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

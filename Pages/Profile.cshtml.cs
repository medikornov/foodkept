using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages
{
    [Authorize(Roles = "Admin, Restaurant, Customer")]
    public class ProfileModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        public string Role { get; set; }

        public async Task OnGetAsync()
        {
            ApplicationUser = await _userManager.GetUserAsync(User);
            Role = _userManager.GetRolesAsync(ApplicationUser).Result[0];
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.Lat = ApplicationUser.Lat;
            user.Lng = ApplicationUser.Lng;
            IdentityResult result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToPage("./Index");

            return RedirectToPage("/Profile");
        }
    }
}

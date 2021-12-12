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

namespace FoodKept.Pages.User
{
    [Authorize(Roles = "Admin, Restaurant, Customer")]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
                return RedirectToPage("../Index");

            return RedirectToPage("/Profile");
        }
    }
}

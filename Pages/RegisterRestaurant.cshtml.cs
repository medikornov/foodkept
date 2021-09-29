using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Models;
using FoodKept.Data;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodKept.Pages
{
    public class RegisterModell : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }

        [BindProperty]
        public RegisterRestaurant RegModell { get; set; }
       
        public RegisterModell(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = RegModell.Email,
                    Email = RegModell.Email,
                    RestaurantName = RegModell.RestaurantName,
                    Country = RegModell.Country,
                    City = RegModell.City,
                    Address = RegModell.Address
                };

                var result = await userManager.CreateAsync(user, RegModell.Password);


                if (!await roleManager.RoleExistsAsync(RegModell.Role))
                {
                    await roleManager.CreateAsync(new IdentityRole(RegModell.Role));
                }
                var assign_role = await userManager.AddToRoleAsync(user, RegModell.Role);


                if (result.Succeeded && assign_role.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD:Pages/RegisterRestaurant.cshtml.cs
using FoodKept.Models;
=======
using FoodKept.Data;
>>>>>>> main:Pages/Register.cshtml.cs
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodKept.Pages
{
    public class RegisterModel : PageModel
    {
<<<<<<< HEAD:Pages/RegisterRestaurant.cshtml.cs
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
=======
        private  UserManager<IdentityUser> userManager { get; }
        private SignInManager<IdentityUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }
>>>>>>> main:Pages/Register.cshtml.cs

        [BindProperty]
        public Register RegModel { get; set; }
       
<<<<<<< HEAD:Pages/RegisterRestaurant.cshtml.cs
        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
=======
        public RegisterModel(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager,
                             RoleManager<IdentityRole> roleManager)
>>>>>>> main:Pages/Register.cshtml.cs
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
                    UserName = RegModel.Email,
                    Email = RegModel.Email,
                    RestaurantName = RegModel.RestaurantName,
                    Country = RegModel.Country,
                    City = RegModel.City,
                    Address = RegModel.Address
                };

                var result = await userManager.CreateAsync(user, RegModel.Password);


                if (!await roleManager.RoleExistsAsync(RegModel.Role))
                {
                    await roleManager.CreateAsync(new IdentityRole(RegModel.Role));
                }
                var assign_role = await userManager.AddToRoleAsync(user, RegModel.Role);


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

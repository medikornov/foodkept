using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodKept.Pages
{
    public class RegisterModel : PageModel
    {
        private  UserManager<IdentityUser> userManager { get; }
        private SignInManager<IdentityUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }

        [BindProperty]
        public Register RegModel { get; set; }
       
        public RegisterModel(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager,
                             RoleManager<IdentityRole> roleManager)
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
                var user = new IdentityUser()
                {
                    UserName = RegModel.Email,
                    Email = RegModel.Email
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

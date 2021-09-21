using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

        [BindProperty]
        public Register RegModel { get; set; }
       
        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

                if (result.Succeeded)
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

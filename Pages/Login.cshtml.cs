using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Models;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodKept.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public Login Model { get; set; }

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var identityResult = await _signInManager.PasswordSignInAsync(Model.Email, Model.Password, Model.RememberMe, false);

                if (identityResult.Succeeded)
                {
                    if(returnUrl == null || returnUrl == "/")
                    {
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        return RedirectToPage(returnUrl);
                    }
                }

                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();
        }
    }
}

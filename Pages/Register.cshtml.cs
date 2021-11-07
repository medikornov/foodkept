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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FoodKept.Helpers;

namespace FoodKept.Pages
{
    [ValidateAntiForgeryToken]
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }

        [BindProperty]
        public RegisterLogin CusRegModel { get; set; }
        [BindProperty]
        public RegisterRestaurant ResRegModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCustomerAsync()
        {
            //Skip all validation to validate specific form only
            ModelState.MarkAllFieldsAsSkipped();

            //Check if all the required field are validated in customer form only
            if (TryValidateModel(CusRegModel, nameof(CusRegModel)))
            {
                var user = new ApplicationUser()
                {
                    UserName = CusRegModel.Email,
                    Email = CusRegModel.Email,
                    FirstName = CusRegModel.FirstName,
                    LastName = CusRegModel.LastName,
                    Country = CusRegModel.Country,
                    City = CusRegModel.City,
                    Address = CusRegModel.Address
                };

                var result = await userManager.CreateAsync(user, CusRegModel.Password);


                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                var assign_role = await userManager.AddToRoleAsync(user, "Customer");


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

        public async Task<IActionResult> OnPostRestaurantAsync()
        {
            //Skip all validation to validate specific form only
            ModelState.MarkAllFieldsAsSkipped();

            //Check if all the required field are validated in restaurant form only
            if (TryValidateModel(ResRegModel, nameof(ResRegModel)))
            {
                var user = new ApplicationUser()
                {
                    UserName = ResRegModel.Email,
                    Email = ResRegModel.Email,
                    RestaurantName = ResRegModel.RestaurantName,
                    Country = ResRegModel.Country,
                    City = ResRegModel.City,
                    Address = ResRegModel.Address
                };

                var result = await userManager.CreateAsync(user, ResRegModel.Password);

                /*var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Register", new { token, email = user.Email }, Request.Scheme);
                /*var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null);
                await _emailSender.SendEmailAsync(message);

                EmailSender emailSender = new EmailSender();
                emailSender.sendEmail(user.Email, confirmationLink);*/

                if (!await roleManager.RoleExistsAsync("Restaurant"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Restaurant"));
                }
                var assign_role = await userManager.AddToRoleAsync(user, "Restaurant");


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
        /*[HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToPage("./Register");
            var result = await userManager.ConfirmEmailAsync(user, token);
            return RedirectToPage("./index");
        }*/
    }


    //Model State Extension method for handling multiple form validation (skips errors)
    public static class ModelStateExtensions
    {
        public static ModelStateDictionary MarkAllFieldsAsSkipped(this ModelStateDictionary modelState)
        {
            foreach (var state in modelState.Select(x => x.Value))
            {
                state.Errors.Clear();
                state.ValidationState = ModelValidationState.Skipped;
            }
            return modelState;
        }
    }

}

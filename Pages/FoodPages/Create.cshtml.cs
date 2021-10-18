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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.ComponentModel.DataAnnotations;
using FoodKept.Extensions;
using FoodKept.ViewModels;

namespace FoodKept.Pages.FoodPages
{
    [Authorize(Roles = "Admin, Restaurant")]
    public class CreateModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(FoodKept.Data.ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Food Food { get; set; }
        [BindProperty]
        [Display(Name = "Image")]
        //[Required(ErrorMessage = "Pick an Image")]
        [AllowedImgExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile FoodImage { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [BindProperty]
        public List<SelectListItem> EnumCategories { get; set; }

        public IActionResult OnGet()
        {
            EnumCategories = Enum.GetValues(typeof(FoodCategories.Category)).Cast<FoodCategories.Category>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = v.ToString()
            }).ToList();

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //If the user creates a food, that food is linked to the user by id
            Food.ApplicationUserId = _userManager.GetUserId(User);

            if (FoodImage != null)
            {
                //Convert image file to byte array and add to model
                Food.FoodImage = GetByteArrayFromImage(FoodImage);
            }

            //Initialize CurrentPrice
            Food.CurrentPrice = new CurrentPrice();

            _context.FoodData.Add(Food);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}

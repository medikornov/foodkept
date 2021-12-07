using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FoodKept.Extensions;
using System.IO;
using FoodKept.ViewModels;

namespace FoodKept.Pages.FoodPages
{
    [Authorize(Roles = "Admin, Restaurant")]
    public class EditModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public EditModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Food Food { get; set; }
        [BindProperty]
        [Display(Name = "Image")]
        [Required(ErrorMessage = "Pick an Image")]
        [AllowedImgExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile FoodImage { get; set; }
        [BindProperty]
        public List<SelectListItem> EnumCategories { get; set; }

        public IActionResult OnGet(int? id)
        {
            //Lazy initialization
            Lazy<Food> getFood = new Lazy<Food>(() => _context.FoodData.FirstOrDefault(m => m.ID == id));

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Food = getFood.Value;
            }

            EnumCategories = Enum.GetValues(typeof(FoodCategories.Category)).Cast<FoodCategories.Category>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = v.ToString()
            }).ToList();

            if (Food == null)
            {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Convert image file to byte array and add to model
            Food.FoodImage = GetByteArrayFromImage(FoodImage);

            _context.Attach(Food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(Food.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FoodExists(int id)
        {
            return _context.FoodData.Any(e => e.ID == id);
        }

        private static byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}

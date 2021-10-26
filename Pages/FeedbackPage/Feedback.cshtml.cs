using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FeedbackPage
{
    public class FeedbackModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;
        public FeedbackModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Feedback Feedback { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.Feedbacks.Add(Feedback);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }

    }
}

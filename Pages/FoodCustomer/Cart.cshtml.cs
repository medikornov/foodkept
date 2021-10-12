using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FoodCustomer
{
    public class CartModel : PageModel
    {
        public IList<ShoppingCart> Cart { get; set; }
        private readonly ShopContext context;
        private readonly UserManager<ApplicationUser> userManager;
        
        public CartModel(ShopContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public void OnGet()
        {
            var userId = userManager.GetUserId(User);
            Cart = context.Cart.Include(c => c.Food).Where(c => c.ApplicationUserId == userId).ToList();
        }

        public async Task<IActionResult> OnPostAddToCart(string id)
        {
            Food food = context.FoodData.FirstOrDefault(db => db.ID.ToString() == id);
            ShoppingCart result = context.Cart.FirstOrDefault(c =>
                    c.ApplicationUserId == userManager.GetUserId(User) &&
                    c.FoodId == food.ID);
            

            if (result != null)
            {
                result.Quantity++;
                context.Attach(result).State = EntityState.Modified;
                await context.SaveChangesAsync();
                
                return new JsonResult(result.Quantity);
            }
            else
            {
                ShoppingCart cart = new ShoppingCart
                {
                    FoodId = food.ID,
                    ApplicationUserId = userManager.GetUserId(User),
                    Quantity = 1
                };

                context.Cart.Add(cart);
                await context.SaveChangesAsync();

                return new JsonResult(cart.Quantity);
            }

        }
        public async Task<IActionResult> OnPostRemoveFromCartAsync(int id)
        {
            var cart = context.Cart.FirstOrDefault(op => op.Id == id);

            if (cart != null)
            {
                context.Cart.Remove(cart);
                await context.SaveChangesAsync();
            }
            OnGet();
            return Page();
        }
    }

}


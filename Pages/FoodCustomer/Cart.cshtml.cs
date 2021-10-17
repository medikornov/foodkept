using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FoodCustomer
{
    [Authorize(Roles = "Customer, Admin")]
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


        public IActionResult OnPostReserve()
        {
            OnGet();
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new System.Net.NetworkCredential("foodkepterino@gmail.com", "foodkept4");

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("foodkepterino@gmail.com", "FoodKept");
            mail.To.Add(new MailAddress("foodkepterino@gmail.com"));
            mail.CC.Add(new MailAddress("foodkepterino@gmail.com"));
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";

            string message = "";
            foreach (var food in Cart)
            {
                message +=
                    "name: " + food.Food.FoodName + "   |  " +
                    "restaurantName: " + food.Food.ApplicationUser.RestaurantName + "   |  " +
                    "price: " + food.Food.Price + "   |  " +
                    "quantity: " + food.Quantity + "<br />";
            }

            mail.Body = message;
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception)
            {

            }
            return Page();
        }

        public async Task<IActionResult> OnPostChangeQuantity(int minus_plus, int id)
        {
            var cart = context.Cart.FirstOrDefault(op => op.Id == id);
            if (cart != null)
            {
                cart.Quantity = minus_plus;
                context.Attach(cart).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            //var cart = context.Cart.FirstOrDefault(op => op.Id)
            return new JsonResult(new {
                minus_plus = minus_plus,
                id = id });
        }
    }

}


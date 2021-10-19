using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Helpers;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CartModel(ShopContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }


        public void OnGet()
        {
            var userId = _userManager.GetUserId(User);
            Cart = _context.Cart.Include(c => c.Food).Where(c => c.ApplicationUserId == userId).ToList();

            foreach(var cartItem in Cart)
            {
                CalculateCurrentPrice.CalculatePriceForFood(cartItem.Food);
            }
        }


        public async Task<IActionResult> OnPostAddToCart(string id)
        {
            Food food = _context.FoodData.FirstOrDefault(db => db.ID.ToString() == id);
            ShoppingCart result = _context.Cart.FirstOrDefault(c =>
                    c.ApplicationUserId == _userManager.GetUserId(User) &&
                    c.FoodId == food.ID);

            if (result != null)
            {
                result.Quantity = (result.Quantity < food.Quantity) ? result.Quantity + 1 : food.Quantity
                    ;
                _context.Attach(result).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                
                return new JsonResult(result.Quantity);
            }
            else
            {
                ShoppingCart cart = new ShoppingCart
                {
                    FoodId = food.ID,
                    ApplicationUserId = _userManager.GetUserId(User),
                    Quantity = 1
                };

                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();

                return new JsonResult(cart.Quantity);
            }

        }
        public async Task<IActionResult> OnPostRemoveFromCartAsync(int id)
        {
            var cart = _context.Cart.FirstOrDefault(op => op.Id == id);

            if (cart != null)
            {
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
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
            mail.Subject = _userManager.GetUserAsync(User).Result.FirstName + " reservation"; 
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";

            string message = "";
            //message = ReadFromFile("")
            string path = Path.Combine(_environment.ContentRootPath, "App_Data\\emailTemplate.txt");
            message = ReadFromFile(path);
            foreach (var food in Cart)
            {
                var foodPrice = food.Food.Price;

                if (food.Food.CurrentPrice.IsDiscount)
                {
                    foodPrice = food.Food.CurrentPrice.DiscountPrice;
                }

                message +=
                    "name: " + food.Food.FoodName + "   |  " +
                    "restaurantName: " + food.Food.ApplicationUser.RestaurantName + "   |  " +
                    "price: " + foodPrice + "   |  " +
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

        private static string ReadFromFile(string path)
        {
            string message;
            try
            {
                StreamReader sr = new StreamReader(path);
                message = sr.ReadLine();
                string newLine;
                while((newLine = sr.ReadLine()) != null)
                {
                    message += newLine;
                }
                sr.Close();
            }
            catch(Exception)
            {
                return "";
            }
            return message;
        }

        public async Task<IActionResult> OnPostChangeQuantity(int minus_plus, int id)
        {
            var cart = _context.Cart.FirstOrDefault(op => op.Id == id);
            if (cart != null)
            {
                cart.Quantity = minus_plus;
                _context.Attach(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            //var cart = context.Cart.FirstOrDefault(op => op.Id)
            return new JsonResult(new {
                minus_plus = minus_plus,
                id = id });
        }
    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
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
            Cart = _context.Cart.Include(c => c.Food)
                .Include(c => c.Food.CurrentPrice)
                .Include(c => c.Food.ApplicationUser)
                .Include(c => c.Food.DiscountList)
                .Where(c => (c.ApplicationUserId == userId) && (c.Reserved == false)).ToList();

            foreach (var cartItem in Cart)
            {
                CalculateCurrentPrice.CalculatePriceForFood(cartItem.Food);
            }
        }


        public async Task<IActionResult> OnPostAddToCart(string id)
        {
            var cartt = Cart;
            Food food = _context.FoodData.FirstOrDefault(db => db.ID.ToString() == id);
            ShoppingCart result = _context.Cart.FirstOrDefault(c =>
                    c.ApplicationUserId == _userManager.GetUserId(User) &&
                    c.FoodId == food.ID &&
                    c.Reserved == false);

            if (result != null)
            {
                result.Quantity = (result.Quantity < food.Quantity) ? result.Quantity + 1 : food.Quantity;
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
                    Quantity = 1,
                    Reserved = false,
                    Customer = await _userManager.GetUserAsync(User)
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


        public async Task <IActionResult> OnPostReserve()
        {
            OnGet();
            foreach (var item in Cart)
            {
                var food = item.Food;
                food.Quantity = item.Food.Quantity - item.Quantity;
                item.Reserved = true;
                _context.Attach(food).State = EntityState.Modified;
                _context.Attach(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new System.Net.NetworkCredential("foodkepterino@gmail.com", "foodkept4");

            var user = _userManager.GetUserAsync(User).Result;

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("foodkepterino@gmail.com", "FoodKept");
            mail.To.Add(new MailAddress(user.Email));
            mail.Subject = user.FirstName + " reservation"; 
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";

            string path = Path.Combine(path1: _environment.ContentRootPath, path2: "App_Data\\emailTemplate.txt");

            var foodsInfo = _context.FoodData.ToList().Join(
                Cart,
                food => food.ID, crt => crt.FoodId,
                (food, crt) => new
                {
                    foodId = food.ID,
                    foodName = food.FoodName,
                    quantity = crt.Quantity,
                    foodsOwner = food.ApplicationUserId
                }).ToList();

            // group join
            var infoForCustomer = _context.ApplicationUsers.ToList().GroupJoin(
                foodsInfo,
                appUser => appUser.Id, foodInfo => foodInfo.foodsOwner,
                (appUser, collection) => new
                {
                    Restaurant = appUser.RestaurantName,
                    Country = appUser.Country,
                    City = appUser.City,
                    Address = appUser.Address,
                    collection = collection
                }).ToList();


            // named argument usage
            string message = ReadFromFile(filePath: path);
            foreach(var restaurants in infoForCustomer)
            {
                if (!restaurants.collection.Any()) continue;
                message +=
                    $"<p>{restaurants.Restaurant}</p>" +
                    $"<p1>Adress: {restaurants.Address}, {restaurants.City}    {restaurants.Country}</p1><br />";

                foreach (var foods in restaurants.collection)
                {
                    message += $"<label>Meal: {foods.foodName}  ||  Quantity: {foods.quantity}</label><br />";
                }
                message += "<br />";
            }

            mail.Body = message;
            try
            {
                smtpClient.Send(message: mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                SmtpStatusCode status = ex.InnerExceptions[0].StatusCode;

                if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                {
                    ViewData["Message"] = string.Format("Delivery failed - retrying again");
                    Thread.Sleep(1000);
                    smtpClient.Send(mail);
                }
                else
                {
                    ViewData["Message"] = string.Format("Failed to deliver message to {0}", ex.InnerExceptions[0].FailedRecipient);
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                ViewData["Message"] = string.Format("Unable to send reservation to your email. Please try again. Error: {0}", ex.ToString());
                return Page();
            }

            OnGet();
            ViewData["Message"] = string.Format("Succesfully reserved your order!");

            return Page();
        }


        private static string ReadFromFile(string filePath)
        {
            string message;
            try
            {
                StreamReader sr = new StreamReader(filePath);
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


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
        public IList<ShoppingCart> cart { get; set; }
        private readonly ShopContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public CartModel(ShopContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            this.context = context;
            this.userManager = userManager;
            this.environment = environment;
        }


        public void OnGet()
        {
            var userId = userManager.GetUserId(User);
            cart = context.Cart.Include(c => c.Food).Where(c => c.ApplicationUserId == userId).ToList();

            foreach(var cartItem in cart)
            {
                CalculateCurrentPrice.CalculatePriceForFood(cartItem.Food);
            }
        }


        public async Task<IActionResult> OnPostAddToCart(string id)
        {
            Food food = context.FoodData.FirstOrDefault(db => db.ID.ToString() == id);
            ShoppingCart result = context.Cart.FirstOrDefault(c =>
                    c.ApplicationUserId == userManager.GetUserId(User) &&
                    c.FoodId == food.ID);

            if (result != null)
            {
                result.Quantity = (result.Quantity < food.Quantity) ? result.Quantity + 1 : food.Quantity;
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
            mail.Subject = userManager.GetUserAsync(User).Result.FirstName + " reservation"; 
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";

            string path = Path.Combine(path1: environment.ContentRootPath, path2: "App_Data\\emailTemplate.txt");

            var foodsInfo = context.FoodData.ToList().Join(
                cart,
                food => food.ID, crt => crt.FoodId,
                (food, crt) => new
                {
                    foodId = food.ID,
                    foodName = food.FoodName,
                    quantity = crt.Quantity,
                    foodsOwner = food.ApplicationUserId
                }).ToList();

            /******************************** Group join *******************************/
            var infoForCustomer = context.ApplicationUsers.ToList().GroupJoin(
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
            catch (Exception)
            {

            }
            return Page();
        }

        // reading from file
        private string ReadFromFile(string filePath)
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


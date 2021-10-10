using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodKept.Pages.FoodCustomer
{
    public class CartModel : PageModel
    {
        public List<Food> cart { get; set; }
        private readonly ShopContext context;
        public Food Food { get; set; }
        public CartModel(ShopContext context)
        {
            this.context = context;
        }
        public void OnGet()
        {
            /*cart = SessionHelper.GetObjectFromJson<List<Food>>(HttpContext.Session, "cart");*/
        }

        public IActionResult OnPostAddToCart(string id)
        {
            Food food = context.FoodData.FirstOrDefault(db => db.ID.ToString() == id);
            Food new_food = new Food
            {
                ID = food.ID,
                FoodName = food.FoodName,
                Price = food.Price,
                Discount = food.Discount,
                FoodImage = food.FoodImage
            };

            return new JsonResult(new_food);
        }
    }
}


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodKept.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodKept.Helpers
{
    public static class CalculateCurrentPrice
    {
        private static readonly DbContextFactory _dbContextFactory;
        static readonly object _lock = new object();
        static CalculateCurrentPrice()
        {
            _dbContextFactory = new DbContextFactory();
        }

        public static async Task<PaginatedList<Food>> CalculatePriceForFoodListAsync(PaginatedList<Food> Food)
        {
            List<Task<Food>> tasks = new List<Task<Food>>();
            var context = _dbContextFactory.Create();

            IsNull(Food);

            foreach (var item in Food)
            {
                var newContextItem = context.FoodData
                    .Include(f => f.CurrentPrice)
                    .Include(f => f.ApplicationUser)
                    .Include(f => f.DiscountList)
                    .FirstOrDefault(m => m.ID == item.ID);
                    
                tasks.Add(Task.Run(() => AddDiscountToFood(newContextItem)));
            }

            var results = await Task.WhenAll(tasks);

            for (int i = 0; i < results.Length; i++)
            {
                Food[i] = results[i];
            }

            return Food;
        }

        public static Food CalculatePriceForFood(Food Food)
        {
            IsNull(Food);

            return AddDiscountToFood(Food);
        }

        private static Food AddDiscountToFood(Food Food)
        {
            //Get current time
            TimeSpan CurrentTime = DateTime.Now.TimeOfDay;

            lock (_lock)
            {
                Food.CurrentPrice.OldPrice = Food.Price;
                Food.RestaurantName = Food.ApplicationUser.RestaurantName;
                Food.CurrentPrice.IsDiscount = false;

                foreach (var discount in Food.DiscountList)
                {
                    if (TimeSpan.Compare(CurrentTime, discount.FromTime) == 1 && TimeSpan.Compare(CurrentTime, discount.ToTime) == -1)
                    {
                        Food.CurrentPrice.DiscountPrice = discount.DiscountPrice;
                        Food.CurrentPrice.DiscountPercent = discount.DiscountPercent;
                        Food.CurrentPrice.IsDiscount = true;
                        Food.Discount = discount.DiscountPercent;

                        break;
                    }
                }

                return Food;
            }
        }

        private static void IsNull(object Object)
        {
            if (Object is null)
            {
                throw new ArgumentNullException(nameof(Object));
            }
        }
    }
}

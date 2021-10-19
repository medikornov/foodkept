// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Models;

namespace FoodKept.Helpers
{
    public static class CalculateCurrentPrice
    {
        public static void CalculatePriceForFoodList(IList<Food> Food)
        {
            //Get current time
            TimeSpan CurrentTime = DateTime.Now.TimeOfDay;

            IsNull(Food);

            foreach (var item in Food)
            {
                AddDiscountToFood(CurrentTime, item);
            }
        }

        public static void CalculatePriceForFood(Food Food)
        {
            //Get current time
            TimeSpan CurrentTime = DateTime.Now.TimeOfDay;

            IsNull(Food);

            AddDiscountToFood(CurrentTime, Food);
        }

        private static void AddDiscountToFood(TimeSpan CurrentTime, Food Food)
        {
            Food.CurrentPrice.OldPrice = Food.Price;
            Food.CurrentPrice.IsDiscount = false;

            foreach (var discount in Food.DiscountList)
            {
                if (TimeSpan.Compare(CurrentTime, discount.FromTime) == 1 && TimeSpan.Compare(CurrentTime, discount.ToTime) == -1)
                {
                    Food.CurrentPrice.DiscountPrice = discount.DiscountPrice;
                    Food.CurrentPrice.DiscountPercent = discount.DiscountPercent;
                    Food.CurrentPrice.IsDiscount = true;

                    break;
                }
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

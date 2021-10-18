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
        public static void CalculatePrice(IList<Food> Food)
        {
            if (Food is null)
            {
                throw new ArgumentNullException(nameof(Food));
            }

            //Get current 
            TimeSpan CurrentTime = DateTime.Now.TimeOfDay;

            foreach (var item in Food)
            {
                item.CurrentPrice.OldPrice = item.Price;
                item.CurrentPrice.IsDiscount = false;

                foreach (var discount in item.DiscountList)
                {
                    if (TimeSpan.Compare(CurrentTime, discount.FromTime) == 1 && TimeSpan.Compare(CurrentTime, discount.ToTime) == -1)
                    {
                        item.CurrentPrice.DiscountPrice = discount.DiscountPrice;
                        item.CurrentPrice.DiscountPercent = discount.DiscountPercent;
                        item.CurrentPrice.IsDiscount = true;

                        break;
                    }
                }
            }
        }
    }
}

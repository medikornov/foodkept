// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodKept.Models
{
    public class Food
    {
        public int FoodID { get; set; }
        public String UserID { get; set; }
        public string FoodName { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }

    }
}

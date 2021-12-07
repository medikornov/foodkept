// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodKept.ViewModels
{
    public class FoodCategories
    {
        [Flags]
        public enum Category
        {
            None = 0,
            Fruit = 1,
            FastFood = 2,
            Pastries = 4,
            Meat = 8,
            Other = 16
        }
    }
}

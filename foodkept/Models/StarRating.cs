// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodKept.Models
{
    public class StarRating
    {
        public int ID { get; set; }
        public int Rate { get; set; }
        public int FoodId { get; set; }
        public virtual Food food { get; set; }
    }
}

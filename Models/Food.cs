// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FoodKept.Models
{
    public class Food
    {
        public int ID { get; set; }
        public string FoodName { get; set; }
        public double Price { get; set; }
        public int Discount { get; set; }
        public byte[] FoodImage { get; set; }
        public virtual string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

}
}

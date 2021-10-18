// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FoodKept.Models;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FoodKept.Models
{
    public class Food
    {
        public int ID { get; set; }
        public string FoodName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string FoodCategory { get; set; }
        public byte[] FoodImage { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual IList<Discount> DiscountList { get; set; }
        public virtual CurrentPrice CurrentPrice { get; set; }
    }
}

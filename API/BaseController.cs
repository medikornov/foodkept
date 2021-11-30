// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FoodKept.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseController(ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var userId = _userManager.GetUserId(User);
            var Cart = _context.Cart.Where(c => c.ApplicationUserId == userId).ToList();

            var query = from cart in _context.Cart
                        where cart.ApplicationUserId == userId && !cart.Reserved
                        select new
                        {
                            id = cart.Id,
                            foodName = cart.Food.FoodName,
                            quantity = cart.Quantity
                        };
            return new JsonResult(JsonConvert.SerializeObject(query));
        }
    }
}

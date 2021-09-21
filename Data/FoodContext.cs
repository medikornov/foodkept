// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Data
{
    public class FoodContext : IdentityDbContext
    {
        public FoodContext (DbContextOptions<FoodContext> options) : base(options)
        {

        }
        //public DbSet<User> Users { get; set; }

  
    }
}

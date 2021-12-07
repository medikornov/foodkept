// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using FoodKept.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Helpers
{
    public class DbContextFactory
    {
        public ShopContext Create()
        {
            var options = new DbContextOptionsBuilder<ShopContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ShopContext-48d14298-664f-4568-b3e9-bc165fc27376;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

            return new ShopContext(options);
        }
    }
}

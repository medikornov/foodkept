// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;

namespace FoodKept.Models
{
    public class SQLFoodRepository : IFoodRepository
    {
        private readonly ShopContext _context;

        public SQLFoodRepository(ShopContext context)
        {
            _context = context;
        }

        public Food Add(Food food)
        {
            _context.FoodData.Add(food);
            _context.SaveChanges();
            return food;
        }

        public Food Delete(int id)
        {
            Food food = _context.FoodData.Find(id);
            if(food != null)
            {
                _context.FoodData.Remove(food);
                _context.SaveChanges();
            }
            return food;
        }

        public IEnumerable<Food> GetAllFood()
        {
            return _context.FoodData;
        }

        public Food GetFood(int id)
        {
            return _context.FoodData.Find(id);
        }

        public Food Update(Food foodChanges)
        {
            var food = _context.FoodData.Attach(foodChanges);
            food.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return foodChanges;
        }
    }
}

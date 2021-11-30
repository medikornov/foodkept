// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodKept.Models
{
    public interface IFoodRepository
    {
        Food GetFood(int id);
        IEnumerable<Food> GetAllFood();
        Food Add(Food food);
        Food Update(Food foodChanges);
        Food Delete(int id);
    }
}

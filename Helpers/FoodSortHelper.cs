// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FoodKept.Models;

namespace FoodKept.Helpers
{
    public class FoodSortHelper
    {
        public Dictionary<string, Func<string, IQueryable<Food>, IQueryable<Food>>> SortCommandHandler { get; set; }

        static Func<string, IQueryable<Food>, IQueryable<Food>> OrderBy = (stringOrder, foodIQ) => foodIQ.OrderBy(stringOrder);
        static Func<string, IQueryable<Food>, IQueryable<Food>> OrderByDescending = (stringOrder, foodIQ) => foodIQ.OrderByDescending(stringOrder);

        public FoodSortHelper()
        {
            SortCommandHandler = new Dictionary<string, Func<string, IQueryable<Food>, IQueryable<Food>>>()
                {
                    {"_FoodName", OrderByDescending},
                    {"_RestaurantName", OrderByDescending},
                    {"RestaurantName", OrderBy},
                    {"_Price", OrderByDescending},
                    {"Price", OrderBy},
                    {"_Discount", OrderByDescending},
                    {"Discount", OrderBy},
                    {"_Quantity", OrderByDescending},
                    {"Quantity", OrderBy},
                    {"_FoodCategory", OrderByDescending},
                    {"FoodCategory", OrderBy},
                    {"FoodName", OrderBy}
                };
        }
    }

    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
    }
}

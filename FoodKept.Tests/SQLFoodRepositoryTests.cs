using FoodKept.Data;
using FoodKept.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FoodKept.Tests
{
    public class SQLFoodRepositoryTests
    {
        private readonly DbContextOptions<ShopContext> options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "ShopDatabase")
                .Options;

        [Fact]
        public void SQLFoodRepository_Add_ShouldAddFoodToDb_WhenGivenFood()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.SaveChanges();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                Food testFood = new Food() { ID = 2, FoodName = "Kale" };
                foodRepository.Add(testFood);
                List<Food> food = context.FoodData.ToList();

                Assert.Equal(2, food.Count);
                Assert.Equal(1, food[0].ID);
                Assert.Equal("Carrot", food[0].FoodName);
                Assert.Equal(2, food[1].ID);
                Assert.Equal("Kale", food[1].FoodName);
            }
        }

        [Fact]
        public void SQLFoodRepository_Delete_ShouldDeleteFood_WhenFoodExists()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.SaveChanges();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                foodRepository.Delete(1);
                List<Food> food = context.FoodData.ToList();

                Assert.Empty(food);
            }
        }

        [Fact]
        public void SQLFoodRepository_Delete_ShouldNotDeleteFood_WhenFoodDoesNotExist()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.SaveChanges();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                foodRepository.Delete(2);
                List<Food> food = context.FoodData.ToList();

                Assert.Single(food);
                Assert.Equal(1, food[0].ID);
                Assert.Equal("Carrot", food[0].FoodName);
            }
        }

        [Fact]
        public void SQLFoodRepository_GetAllFood_ShouldReturnFood_WhenFoodExists()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.FoodData.Add(new Food { ID = 2, FoodName = "Pizza" });
                context.FoodData.Add(new Food { ID = 3, FoodName = "Burger" });
                context.SaveChanges();
            }

            //Assert
            using(var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                List<Food> food = foodRepository.GetAllFood().ToList();

                Assert.Equal(3, food.Count);
                Assert.Equal(1, food[0].ID);
                Assert.Equal("Carrot", food[0].FoodName);
                Assert.Equal(2, food[1].ID);
                Assert.Equal("Pizza", food[1].FoodName);
                Assert.Equal(3, food[2].ID);
                Assert.Equal("Burger", food[2].FoodName);
            }
        }

        [Fact]
        public void SQLFoodRepository_GetAllFood_ShouldReturnEmpty_WhenFoodDoesNotExist()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                //Empty database or food table
                context.Database.EnsureDeleted();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                IList<Food> food = foodRepository.GetAllFood().ToList();

                Assert.NotNull(food);
                Assert.Equal(0, food.Count);
            }
        }

        [Fact]
        public void SQLFoodRepository_GetFood_ShouldReturnFood_WhenFoodExists()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.FoodData.Add(new Food { ID = 2, FoodName = "Pizza" });
                context.FoodData.Add(new Food { ID = 3, FoodName = "Burger" });
                context.SaveChanges();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                Food food = foodRepository.GetFood(2);

                Assert.NotNull(food);
                Assert.Equal(2, food.ID);
                Assert.Equal("Pizza", food.FoodName);
            }
        }

        [Fact]
        public void SQLFoodRepository_GetFood_ShouldReturnNull_WhenFoodDoesNotExist()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                Food food = foodRepository.GetFood(1);

                Assert.Null(food);
            }
        }

        [Fact]
        public void SQLFoodRepository_Update_ShouldUpdateDb_WhenFoodExists()
        {
            //Arange
            using (var context = new ShopContext(options))
            {
                context.Database.EnsureDeleted();
                context.FoodData.Add(new Food { ID = 1, FoodName = "Carrot" });
                context.SaveChanges();
            }

            //Assert
            using (var context = new ShopContext(options))
            {
                SQLFoodRepository foodRepository = new SQLFoodRepository(context);
                Food food = context.FoodData.Find(1);

                food.FoodName = "NotCarrot";
                foodRepository.Update(food);
                Food newFood = context.FoodData.Find(1);

                Assert.NotNull(newFood);
                Assert.Equal("NotCarrot", newFood.FoodName);
            }
        }
    }
}

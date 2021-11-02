using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodKept.Data;
using FoodKept.Models;
using FoodKept.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FoodPages
{
    public class ReservationsModel : PageModel
    {
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IList<ShoppingCart> cart;
        public ReservationsModel(ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task OnGetAsync()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            
            var restaurantsFoods = applicationUser.FoodList;
            var result = from a in restaurantsFoods
                         join b in _context.Cart.ToList()
                         on a.ID equals b.FoodId
                         where b.Reserved = true
                         select b;
            cart = result.ToList();
        }
    }
}

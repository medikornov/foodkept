using FoodKept.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodKept.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        public void OnGet()
        {
            /*var user = userManager.GetUserAsync(HttpContext.User).Result;
            name = user.UserName;
            if (user != null)
            {
                role = userManager.GetRolesAsync(user).Result[0];
            }
            //var user = userManager.GetUserName(HttpContext.User);
            //var role = userManager.GetRolesAsync(user);
            */
        }
    }
}

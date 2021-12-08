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
        private readonly Serilog.ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(Serilog.ILogger logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        public void OnGet()
        {
           
        }

    }
}

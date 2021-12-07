using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoodKept.Pages.FoodPages
{
    [Authorize(Roles = "Admin, Restaurant")]
    public class DetailsModel : PageModel
    {
        private readonly IFoodRepository _foodRepository;

        public DetailsModel(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public Food Food { get; set; }

        public void OnGet(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            Food = _foodRepository.GetFood((int)id);

            if (Food == null)
            {
                NotFound();
            }
            Page();
        }
    }
}

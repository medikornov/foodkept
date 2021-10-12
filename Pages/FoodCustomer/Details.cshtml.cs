using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using FoodKept.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodKept.Pages.FoodCustomer
{
    [Authorize(Roles = "Admin, Restaurant, Customer")]
    public class DetailsModel : PageModel
    {
        private readonly FoodKept.Data.ShopContext _context;

        public DetailsModel(FoodKept.Data.ShopContext context)
        {
            _context = context;
        }

        public Food Food { get; set; }
        public static XElement Lat { get; set; }
        public static XElement Lng { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Food = await _context.FoodData.FirstOrDefaultAsync(m => m.ID == id);

            if (Food == null)
            {
                return NotFound();
            }

            //Configure google map api
            //string address = Food.ApplicationUser.Address;
            //string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), "AIzaSyARsUsTCeN6f-yW0mzaLbz03vnNcmAiK58");

            //WebRequest request = WebRequest.Create(requestUri);
            //WebResponse response = request.GetResponse();
            //XDocument xdoc = XDocument.Load(response.GetResponseStream());
            //Console.WriteLine(response);

            //XElement result = xdoc.Element("GeocodeResponse").Element("result");
            //XElement locationElement = result.Element("geometry").Element("location");
            //Lat = locationElement.Element("lat");
            //Lng = locationElement.Element("lng");

            return Page();
        }
    }
}

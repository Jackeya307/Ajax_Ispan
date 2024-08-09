using Ajax_Lab01.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using System.Diagnostics;

namespace Ajax_Lab01.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private MyDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,MyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var c = _dbContext.Addresses;
            return View(c);
        }

        //todo 根據 city 讀出鄉鎮區(site_id)的資料

        [HttpGet("/Home/Site/{City?}")] //
        public IActionResult Site(string? City)
        {
            //var c = _dbContext.Addresses.Where(Address => Address.SiteId.Contains(City)).Select(Address => Address.SiteId).Distinct().ToList();
            var c = _dbContext.Addresses
                .Where(Address => Address.SiteId.Contains(City))
                .Select(Address => Address.SiteId)
                .Distinct()
                .ToList();
            if(!c.Any()) {
                return NotFound("查無相符的城市，故無法顯示其鄉鎮區");
            }

            return Json(c);
        }
        //todo 根據 site_id 讀出路名(road) 
        [HttpGet("/Home/road/{site_id?}")] //
        public IActionResult road(string? site_id)
        {
            var c = _dbContext.Addresses
                .Where(q => q.SiteId == site_id)
                .Select(s => s.Road)
                .Distinct().ToList();

            return Json(c);
        }


        public IActionResult h123()
        {
            return Content("Ajax, 您好");
        }

        public IActionResult Privacy(int id = 1)
        {
            //IQueryable<string?> context = _dbContext.Addresses.Select(c=>c.City).Distinct();
            //return Json(context);
            var Img = _dbContext.Members.Find(id);
            if(Img != null) {
                Byte[]? memimg = Img.FileData;
                return File(memimg,"image/jpeg");
            }
            return NotFound();
        }

        public IActionResult Category()
        {
            return View(_dbContext.Categories);
        }
        public IActionResult homework()
        {
            return View();
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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
            var b = _dbContext.Addresses.Select(s => s.SiteId).Distinct();
            IQueryable<string?> c = _dbContext.Addresses.Select(s => s.City).Distinct().Concat(b)
                ;
            return View(c);
        }

        //todo 根據 city 讀出鄉鎮區(site_id)的資料
        
        public IActionResult site(string? city = "金門縣")
        {
            if(city != null) {
                IQueryable<string?> c = _dbContext.Addresses.Where(q => q.SiteId.Contains(city)).Select(s => s.SiteId).Distinct();
                return Json(c.ToList());
            }
            return NotFound();
        }
        //todo 根據 site_id 讀出路名(road) 
        public IActionResult road(string? site_id = "高雄市茄萣區")
        {
            IQueryable<string?> c = _dbContext.Addresses.Where(q => q.Road.Contains(site_id)).Select(s => s.Road).Distinct();
            return Json(c.ToList());
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
            return NotFound("沒照片");
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

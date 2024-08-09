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

        //todo �ھ� city Ū�X�m���(site_id)�����

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
                return NotFound("�d�L�۲Ū������A�G�L�k��ܨ�m���");
            }

            return Json(c);
        }
        //todo �ھ� site_id Ū�X���W(road) 
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
            return Content("Ajax, �z�n");
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

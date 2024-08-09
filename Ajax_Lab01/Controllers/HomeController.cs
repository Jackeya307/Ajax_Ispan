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

        //todo �ھ� city Ū�X�m���(site_id)�����
        
        public IActionResult site(string? city = "������")
        {
            if(city != null) {
                IQueryable<string?> c = _dbContext.Addresses.Where(q => q.SiteId.Contains(city)).Select(s => s.SiteId).Distinct();
                return Json(c.ToList());
            }
            return NotFound();
        }
        //todo �ھ� site_id Ū�X���W(road) 
        public IActionResult road(string? site_id = "�������X�_��")
        {
            IQueryable<string?> c = _dbContext.Addresses.Where(q => q.Road.Contains(site_id)).Select(s => s.Road).Distinct();
            return Json(c.ToList());
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
            return NotFound("�S�Ӥ�");
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

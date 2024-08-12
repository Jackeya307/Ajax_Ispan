using Ajax_Lab01.Models;
using Ajax_Lab01.Models.DTO;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace Ajax_Lab01.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private MyDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger,MyDbContext dbContext,IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Cors(){ return View(); }

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

        public IActionResult IspanLoginPratice()
        {
            return View();
        }
        public IActionResult Register(userInfo p)
        {
            //string info = $"{p.userPhoto.Length} {p.userPhoto.FileName}";

            string strPath = $"{_webHostEnvironment.WebRootPath}/images/{p.userPhoto.FileName}";

            using(var fileStream = new FileStream(strPath,FileMode.Create)) {
                p.userPhoto.CopyTo(fileStream);
            }

            Byte[]? img = null;
            using(var memoryStream = new MemoryStream()) {
                p.userPhoto.CopyTo(memoryStream);
                img = memoryStream.ToArray();
            }

            Member member = new Member {
                Name = p.userName,
                //Age = p.userAge,
                Email = p.userEmail,
                Password = p.userPwd,
                FileName = p.userPhoto.FileName,
                FileData = img
            };

            _dbContext.Members.Add(member);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult spots([FromBody] SearchDTO dTO)
        {
            try {

                //根據分類編號讀取相關景點
                var spot = dTO.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots
                    .Where(s => s.CategoryId == dTO.categoryId);

                //關鍵字搜尋
                if(!string.IsNullOrEmpty(dTO.keyword)) {
                    spot = spot.Where(s => s.SpotTitle.Contains(dTO.keyword) || s.SpotDescription.Contains(dTO.keyword));
                }

                switch(dTO.sortBy) {
                    case "spotTitle":
                    spot = dTO.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == dTO.categoryId).OrderByDescending(c => c.SpotTitle);
                    break;
                    case "categoryId":
                    spot = dTO.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == dTO.categoryId).OrderByDescending(c => c.CategoryId);
                    break;
                    default:
                    spot = dTO.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == dTO.categoryId);
                    break;

                }
                //根據價錢搜尋
                int Count = spot.Count();
                int Pages = dTO.page;
                int PageSize = dTO.pageSize;
                int TotalPages = (int)Math.Ceiling((decimal)(Count / PageSize));
                spot = spot.Skip((Pages - 1) * PageSize).Take(PageSize);
                //根據日期區間搜尋
                SpotPagingDTO PagingDTO = new SpotPagingDTO();
                PagingDTO.Pages = Pages;
                PagingDTO.SpotResult = spot.ToList();
                return Json(PagingDTO);
            }
            catch(Exception ex) {
                return NotFound(new {
                    Error = "後端出事了!! "+ex.Message,
                });
            }
        }

        public IActionResult travel()
        {
            return View();
        }



    }



    public class userInfo {
        public string userName {
            get;
            set;
        }
        public string userPwd {
            get;
            set;
        }
        public string userEmail {
            get;
            set;
        }
        public IFormFile? userPhoto {
            get;
            set;
        }
    }
}

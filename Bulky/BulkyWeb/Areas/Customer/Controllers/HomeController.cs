using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _webHostEnvironment; // allows me to save files

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository category, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = category;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _productRepo.GetAll().ToList();
            List<Category> objListCategory = _categoryRepo.GetAll().ToList();
            List<Product> newListProduct = new List<Product>();
            foreach (Product obj in objProductList.ToList())
            {

                var objCategoty = _categoryRepo.GetAll().Where(c => c.Id == obj.CategoryId);

                obj.Category.Id = objCategoty.FirstOrDefault().Id;
                obj.Category.Name = objCategoty.FirstOrDefault().Name;
                obj.Category.DisplayOrder = objCategoty.FirstOrDefault().DisplayOrder;
                newListProduct.Add(obj);


            }

            return View(newListProduct);
        }

        public IActionResult Details(int productId)
        {
            Product objProduct = _productRepo.GetAll().Where(j=>j.Id == productId).FirstOrDefault();
            Category objCategory = _categoryRepo.GetAll().Where(j => j.Id == objProduct.CategoryId).FirstOrDefault();
            Product newProduct = new Product();
            objProduct.Category.Id = objCategory.Id;
            objProduct.Category.Name = objCategory.Name;
            objProduct.Category.DisplayOrder = objCategory.DisplayOrder;


            return View(objProduct);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

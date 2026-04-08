using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;




//using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _webHostEnvironment; // allows me to save files
        public ProductController(IProductRepository db, ICategoryRepository category, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = db;
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
            //return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _categoryRepo.GetAll().Select(j => new SelectListItem
                {
                    Text = j.Name,
                    Value = j.Id.ToString()
                }),
                Product = new Product()
            };

            if (id==null || id == 0) 
            {
                //insert/Create
               
                return View(productVM);
            }
            else
            {
                //Edit
                productVM.Product = _productRepo.Get(j => j.Id == id);
                return View(productVM);
            }
                
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (productVM.Product.Title == productVM.Product.Description.ToString())
            {
                ModelState.AddModelError("description", "The Discription cannot exactly match the Tile");
            }
            if (ModelState.IsValid)
            {
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                if(file != null) 
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootpath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) {
                        //delete de old image, and save the new one
                        var oldImagePath = Path.Combine(wwwRootpath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) 
                    { 
                        // this will copy the file to the location that I created
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.ImageUrl == "" || productVM.Product.ImageUrl == null)
                {
                    //ModelState.AddModelError("ImageUrl", "Invalid Image Url");
                    TempData["error"] = "Problem with the ImageUrl";
                }
                else
                {
                    if (productVM.Product.Id == 0)
                    {

                        _productRepo.Add(productVM.Product);
                        _productRepo.Save();
                        TempData["success"] = "Product created successfuly";

                    }
                    else
                    {
                        // if obj.id ==0, this function will create a new item on the table
                        if (productVM.Product.ImageUrl == "" || productVM.Product.ImageUrl == null)
                        {
                            ModelState.AddModelError("ImageUrl", "Invalid Image Url");
                        }
                        else
                        {
                            _productRepo.Update(productVM.Product);
                            _productRepo.Save();
                            TempData["success"] = "Product updated successfuly";
                        }

                        // Se o Index estiver dentro de um controller diferente como o "Category"
                        //return RedirectToAction("Index","Category");
                        //return RedirectToAction("Index");

                    }
                }



                    // Se o Index estiver dentro de um controller diferente como o "Category"
                    //return RedirectToAction("Index","Category");
                    return RedirectToAction("Index");

            }
            else
            {
                productVM.CategoryList = _categoryRepo.GetAll().Select(j => new SelectListItem
                {
                    Text = j.Name,
                    Value = j.Id.ToString()
                });

                return View(productVM);
            }


        }

        public IActionResult Create()
        {
            ProductVM productVM = new() 
            {   
                CategoryList = _categoryRepo.GetAll().Select(j=> new SelectListItem 
                {
                    Text = j.Name,
                    Value = j.Id.ToString()
                }),
                Product = new Product()
            };


            //IEnumerable<SelectListItem> CategoryList = _categoryRepo
            //    .GetAll().Select(j => new SelectListItem
            //    {
            //        Text = j.Name,
            //        Value = j.Id.ToString()
            //    });

            ////ViewBag.CategoryList = CategoryList; 1.1
            //ViewData["CategoryList"] = CategoryList;
            //ProductVM productVM = new() { 
            //    CategoryList = CategoryList,
            //    Product = new Product()
            
            //};
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (productVM.Product.Title == productVM.Product.Description.ToString())
            {
                ModelState.AddModelError("description", "The Discription cannot exactly match the Tile");
            }
            if (ModelState.IsValid)
            {
                _productRepo.Add(productVM.Product);
                _productRepo.Save();
                //tempData ( will be shown on the page temporarily
                TempData["success"] = "Product created successfuly";
                // Se o Index estiver dentro de um controller diferente como o "Category"
                //return RedirectToAction("Index","Category");
                return RedirectToAction("Index");

            }
            else {
                productVM.CategoryList = _categoryRepo.GetAll().Select(j => new SelectListItem
                {
                    Text = j.Name,
                    Value = j.Id.ToString()
                });

                return View(productVM);
            }
               

        }
        public IActionResult Edit(int? id)
        {
            if(id== null || id == 0)
            {
                return NotFound();
            }

            Product? product = _productRepo.Get(e=> e.Id == id);
            // this options works for all collomns
            // var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            // var category1 = _db.Categories.Where(c=> c.Id== id).FirstOrDefault();

            // this options is for primary key only
            //var category = _db.Categories.Find(id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                // if obj.id ==0, this function will create a new item on the table
                _productRepo.Update(obj);
                _productRepo.Save();
                TempData["success"] = "Product updated successfuly";
                // Se o Index estiver dentro de um controller diferente como o "Category"
                //return RedirectToAction("Index","Category");
                return RedirectToAction("Index");

            }
            return View();

        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    // this options works for all collomns
        //    var product = _productRepo.Get(c => c.Id == id);
        //    // var category1 = _db.Categories.Where(c=> c.Id== id).FirstOrDefault();

        //    // this options is for primary key only
        //    //var category = _db.Categories.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product obj = _productRepo.Get(e=> e.Id ==id);
        //    if (obj == null) 
        //    {
        //        return NotFound();
        //    }
        //    _productRepo.Remove(obj);
        //    _productRepo.Save();
        //    TempData["success"] = "Product deleted successfuly";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
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
            return Json(new {data = newListProduct });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productToBedeleted = _productRepo.Get(c => c.Id == id);
            if (productToBedeleted == null) 
            {
                return Json(new {success = false, message = "Error While deleting"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBedeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);

            }
            _productRepo.Remove(productToBedeleted);
            _productRepo.Save();

            //string wwwRootpath = _webHostEnvironment.WebRootPath;
            //if (file != null)
            //{
            //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            //    string productPath = Path.Combine(wwwRootpath, @"images\product");
            //    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
            //    {
            //delete de old image, and save the new one
            /// var oldImagePath = Path.Combine(wwwRootpath, productVM.Product.ImageUrl.TrimStart('\\'));

            //}


            return Json(new { success = true, message = "Delete Successful" });
        }



        #endregion
    }
}

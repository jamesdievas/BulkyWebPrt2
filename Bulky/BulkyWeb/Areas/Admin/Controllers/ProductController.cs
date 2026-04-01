using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;




//using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        
        public ProductController(IProductRepository db, ICategoryRepository category)
        {
            _productRepo = db;
            _categoryRepo = category;


        }
        public IActionResult Index()
        {
            List<Product> objProductList = _productRepo.GetAll().ToList();

            return View(objProductList);
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

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // this options works for all collomns
            var product = _productRepo.Get(c => c.Id == id);
            // var category1 = _db.Categories.Where(c=> c.Id== id).FirstOrDefault();

            // this options is for primary key only
            //var category = _db.Categories.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product obj = _productRepo.Get(e=> e.Id ==id);
            if (obj == null) 
            {
                return NotFound();
            }
            _productRepo.Remove(obj);
            _productRepo.Save();
            TempData["success"] = "Product deleted successfuly";
            return RedirectToAction("Index");
        }
    }
}

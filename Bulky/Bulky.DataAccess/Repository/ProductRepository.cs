using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Product obj)
        {
            _db.Products.Update(obj);
            // outra alternativa seria validar a url aqui.
            //var objFromDb = _db.Products.FirstOrDefault(j => j.Id == obj.Id);
            //if (objFromDb != null) 
            //{
            //    objFromDb.Title = obj.Title;
            //    objFromDb.ISBN = obj.ISBN;
            //    objFromDb.Author = obj.Author;
            //    objFromDb.Description = obj.Description;
            //    objFromDb.CategoryId = obj.CategoryId;
            //    objFromDb.Price = obj.Price;
            //    objFromDb.ListPrice = obj.ListPrice;
            //    objFromDb.Price100 = obj.Price100;
            //    objFromDb.Price50 = obj.Price50;

            //    if(obj.ImageUrl != null)
            //    {
            //        objFromDb.ImageUrl = obj.ImageUrl;
                
            //    }
            //}
        }
    }
}

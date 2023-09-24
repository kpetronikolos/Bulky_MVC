using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
     [Area("Admin")]
     [Authorize(Roles = SD.Role_Admin)]
     public class ProductController : Controller
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IWebHostEnvironment _webHostEnvironment;

          public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
          {
               _unitOfWork = unitOfWork;
               _webHostEnvironment = webHostEnvironment;
          }
          public IActionResult Index()
          {
               List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();  

               return View(objProductList);
          }

          public IActionResult Upsert(int? id)
          {
               ProductVM productVM = new()
               {
                    CategoryList =  _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                         Text = u.Name,
                         Value = u.Id.ToString()
                    }),
                    Product = new Product()
               };

               if (id > 0) productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);

               return View(productVM);
          }

          [HttpPost]
          public IActionResult Upsert(ProductVM productVM, IFormFile? file)
          {
               if (!ModelState.IsValid)
               {
                    productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                         Text = u.Name,
                         Value = u.Id.ToString()
                    });

                    return View(productVM);
               }

               string wwwRootPath = _webHostEnvironment.WebRootPath;
               if (file != null)
               {
                    string filename = $"{ Guid.NewGuid().ToString() }{ Path.GetExtension(file.FileName) }";
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!String.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                         // Delete the old image
                         string oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                         if (System.IO.File.Exists(oldImagePath))
                         {
                              System.IO.File.Delete(oldImagePath);
                         }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                         file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @$"\images\product\{filename}";
               }

               if (productVM.Product.Id == 0)
               {
                    _unitOfWork.Product.Add(productVM.Product);
               }
               else
               {
                    _unitOfWork.Product.Update(productVM.Product);
               }
               
               _unitOfWork.Save();
               TempData["success"] = "Product created successfully";
               return RedirectToAction("Index");
          }         

          #region API CALLS

          [HttpGet]
          public IActionResult GetAll()
          {
               List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
               return Json(new { data = objProductList });
          }

          [HttpDelete]
          public IActionResult Delete(int? id)
          {
               var productToBeLeted = _unitOfWork.Product.Get(u => u.Id == id);
               if (productToBeLeted == null)
               {
                    return Json(new { success = false, message = "Error while deleting" });
               }

               // Delete the image
               string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeLeted.ImageUrl.TrimStart('\\'));

               if (System.IO.File.Exists(oldImagePath))
               {
                    System.IO.File.Delete(oldImagePath);
               }

               _unitOfWork.Product.Remove(productToBeLeted);
               _unitOfWork.Save();

               return Json(new { success = true, message = "Delete successful" });

          }

          #endregion
     }
}

using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
     [Area("Admin")]
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
               List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();  

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

               _unitOfWork.Product.Add(productVM.Product);
               _unitOfWork.Save();
               TempData["success"] = "Product created successfully";
               return RedirectToAction("Index");
          }          

          public IActionResult Delete(int? id)
          {
               if (id == null || id == 0)
               {
                    return NotFound();
               }

               var product = _unitOfWork.Product.Get(u => u.Id == id);

               if (product == null)
               {
                    return NotFound();
               }

               return View(product);
          }

          [HttpPost, ActionName("Delete")]
          public IActionResult DeletePost(int? id)
          {
               var product = _unitOfWork.Product.Get(u => u.Id == id);

               if (product == null)
               {
                    return NotFound();
               }

               _unitOfWork.Product.Remove(product);
               _unitOfWork.Save();
               TempData["success"] = "Product deleted successfully";
               return RedirectToAction("Index");
          }
     }
}

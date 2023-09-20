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

          public ProductController(IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
          }
          public IActionResult Index()
          {
               List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();  

               return View(objProductList);
          }

          public IActionResult Create()
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

               return View(productVM);
          }

          [HttpPost]
          public IActionResult Create(ProductVM productVM)
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

               _unitOfWork.Product.Add(productVM.Product);
               _unitOfWork.Save();
               TempData["success"] = "Product created successfully";
               return RedirectToAction("Index");
          }

          public IActionResult Edit(int? id)
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

          [HttpPost]
          public IActionResult Edit(Product product)
          {
               if (!ModelState.IsValid)
               {
                    return View();
               }

               _unitOfWork.Product.Update(product);
               _unitOfWork.Save();
               TempData["success"] = "Product updated successfully";
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

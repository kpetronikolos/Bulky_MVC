using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
     [Area("Admin")]
     public class CategoryController : Controller
     {
          private readonly IUnitOfWork _unitOfWork;

          public CategoryController(IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
          }
          public IActionResult Index()
          {
               List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
               return View(objCategoryList);
          }

          public IActionResult Create()
          {
               return View();
          }

          [HttpPost]
          public IActionResult Create(Category category)
          {
               if (!ModelState.IsValid)
               {
                    return View();
               }

               _unitOfWork.Category.Add(category);
               _unitOfWork.Save();
               TempData["success"] = "Category created successfully";
               return RedirectToAction("Index");
          }

          public IActionResult Edit(int? id)
          {
               if (id == null || id == 0)
               {
                    return NotFound();
               }

               var category = _unitOfWork.Category.Get(u => u.Id == id);

               if (category == null)
               {
                    return NotFound();
               }

               return View(category);
          }

          [HttpPost]
          public IActionResult Edit(Category category)
          {
               if (!ModelState.IsValid)
               {
                    return View();
               }

               _unitOfWork.Category.Update(category);
               _unitOfWork.Save();
               TempData["success"] = "Category updated successfully";
               return RedirectToAction("Index");
          }

          public IActionResult Delete(int? id)
          {
               if (id == null || id == 0)
               {
                    return NotFound();
               }

               var category = _unitOfWork.Category.Get(u => u.Id == id);

               if (category == null)
               {
                    return NotFound();
               }

               return View(category);
          }

          [HttpPost, ActionName("Delete")]
          public IActionResult DeletePost(int? id)
          {
               var category = _unitOfWork.Category.Get(u => u.Id == id);

               if (category == null)
               {
                    return NotFound();
               }

               _unitOfWork.Category.Remove(category);
               _unitOfWork.Save();
               TempData["success"] = "Category deleted successfully";
               return RedirectToAction("Index");
          }
     }
}

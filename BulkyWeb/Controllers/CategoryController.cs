using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRepo;

		public CategoryController(ICategoryRepository db)
		{
               _categoryRepo = db;
		}
		public IActionResult Index()
		{
			List<Category> objCategoryList = _categoryRepo.GetAll().ToList();	
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

               _categoryRepo.Add(category);
               _categoryRepo.Save();
			TempData["success"] = "Category created successfully";
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _categoryRepo.Get(u => u.Id == id);

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

               _categoryRepo.Add(category);
               _categoryRepo.Save();
               TempData["success"] = "Category updated successfully";
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _categoryRepo.Get(u => u.Id == id);

               if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			var category = _categoryRepo.Get(u => u.Id == id);

               if (category == null)
			{
				return NotFound();
			}

			_categoryRepo.Remove(category);
			_categoryRepo.Save();
			TempData["success"] = "Category deleted successfully";
			return RedirectToAction("Index");
		}
	}
}

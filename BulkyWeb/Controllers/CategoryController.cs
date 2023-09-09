﻿using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _db;

		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			List<Category> objCategoryList = _db.Categories.ToList();	
			return View(objCategoryList);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category category)
		{
			//if (category.Name == category.DisplayOrder.ToString())
			//{
			//	ModelState.AddModelError("Name", "The Display Order cannot exactly match the Name.");
			//}

			if (!ModelState.IsValid)
			{
				return View();
			}

			_db.Categories.Add(category);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _db.Categories.Find(id);

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

			_db.Categories.Update(category);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			var category = _db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}

			_db.Categories.Remove(category);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}

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
			if (category.Name == category.DisplayOrder.ToString())
			{
				ModelState.AddModelError("Name", "The Display Order cannot exactly match the Name.");
			}

			if (!ModelState.IsValid)
			{
				return View();
			}

			_db.Categories.Add(category);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}

using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	public class BlogsController : Controller
	{
		private BloggingContext _context;
		private ILogger<BlogsController> _logger;

		public BlogsController(BloggingContext context, ILogger<BlogsController> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View(_context.Blog.ToList());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Edit(int id, string url)
		{
			_logger.LogInformation(0, $"id = {id} url = {(url ?? "<null>")}");
			if (id <= 0) return Json("error");

			Blog person = _context.Blog.First(p => p.BlogId == id);
			person.Url = url;
			_context.SaveChanges();

			return Json(new { person });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Blog blog)
		{
			if (ModelState.IsValid)
			{
				_context.Blog.Add(blog);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(blog);
		}

	}
}

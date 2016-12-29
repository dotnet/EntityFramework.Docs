using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

		public async Task<IActionResult> Index()
		{
			var lst = _context.Blog.ToAsyncEnumerable();

			return View(await lst.ToList());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> Edit(int id, string url)
		{
			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(0, $"id = {id} url = {(url ?? "<null>")}");
			});

			if (id <= 0) return Json("error");

			Task<Blog> tsk = _context.Blog.Where(p => p.BlogId == id).ToAsyncEnumerable().FirstOrDefault();
			Blog blog = await tsk;
			blog.Url = url;

			await _context.SaveChangesAsync();
			//await logger_tsk;

			return Json(blog);
		}

		[HttpDelete]
		//[ValidateAntiForgeryToken]
		public async Task<JsonResult> Delete(int id)
		{
			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(2, $"id = {id}");
			});

			if (id <= 0) return Json("error");

			Task<Blog> tsk = _context.Blog.Where(p => p.BlogId == id).ToAsyncEnumerable().FirstOrDefault();
			Blog blog = await tsk;
			_context.Remove(blog);

			await _context.SaveChangesAsync();
			//await logger_tsk;

			return Json("ok");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Blog blog)
		{
			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(1, $"id = {blog.BlogId} url = {(blog.Url ?? "<null>")}");
			});

			if (ModelState.IsValid)
			{
				await _context.Blog.AddAsync(blog);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			return View(blog);
		}

	}
}

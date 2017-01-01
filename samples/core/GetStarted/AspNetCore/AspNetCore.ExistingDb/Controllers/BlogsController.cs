using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	public class BlogsController : Controller
	{
		private BloggingContext _context;
		private ILogger<BlogsController> _logger;
		private IConfiguration _conf;

		public BlogsController(BloggingContext context, ILogger<BlogsController> logger, IConfiguration conf)
		{
			_context = context;
			_logger = logger;
			_conf = conf;
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
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> Edit(int id, string url)
		{
			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(0, $"id = {id} url = {(url ?? "<null>")}");
			});

			if (id <= 0 || string.IsNullOrEmpty(url)) return Json("error0");

			Task<Blog> tsk = _context.Blog.FindAsync(id);
			Blog blog = await tsk;
			if (blog != null && url != blog.Url)
			{
				blog.Url = url;
				await _context.SaveChangesAsync();

				return Json(blog);
			}

			return Json("error1");
		}

		[HttpDelete]
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> Delete(int id)
		{
			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(2, $"id = {id}");
			});

			if (id <= 0) return Json("error0");

			Task<Blog> tsk = _context.Blog.FindAsync(id);
			Blog blog = await tsk;
			if (blog != null)
			{
				_context.Remove(blog);
				await _context.SaveChangesAsync();

				return Json("ok");
			}
			else
				return Json("error1");
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
				var appRootPath = _conf["AppRootPath"];

				await _context.Blog.AddAsync(blog);
				await _context.SaveChangesAsync();

				var route = appRootPath + "Blogs";
				return Redirect(route);
			}

			return View(blog);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	public class HashesController : Controller
	{
		private BloggingContext _dbaseContext;
		private ILogger<BlogsController> _logger;

		public HashesController(BloggingContext context, ILogger<BlogsController> logger)
		{
			_dbaseContext = context;
			_logger = logger;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Search(string search, string shaKind)
		{
			Hashes found = null;
			if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(shaKind))
				return Json(found);

			_logger.LogInformation(0, $"{nameof(search)} = {search}, {nameof(shaKind)} = {shaKind}");

			search = search.Trim().ToLower();
			switch (shaKind)
			{
				case "MD5":
				case "md5":
					found = _dbaseContext.Hashes.FirstOrDefault(x => x.HashMD5 == search);
					break;

				case "SHA256":
				case "sha256":
					found = _dbaseContext.Hashes.FirstOrDefault(x => x.HashSHA256 == search);
					break;

				default:
					throw new NotSupportedException("bad kind");
			}

			return Json(found);
		}
	}
}

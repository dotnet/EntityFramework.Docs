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
		private ILogger<HashesController> _logger;

		public HashesController(BloggingContext context, ILogger<HashesController> logger)
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
		public async Task<JsonResult> Search(string search, string shaKind)
		{
			if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(shaKind))
				return null;

			var logger_tsk = Task.Run(() =>
			{
				_logger.LogInformation(0, $"{nameof(search)} = {search}, {nameof(shaKind)} = {shaKind}");
			});

			search = search.Trim().ToLower();
			Task<Hashes> found = null;
			switch (shaKind)
			{
				case "MD5":
				case "md5":
					found = _dbaseContext.Hashes.Where(x => x.HashMD5 == search).ToAsyncEnumerable().FirstOrDefault();
					break;

				case "SHA256":
				case "sha256":
					found = _dbaseContext.Hashes.Where(x => x.HashSHA256 == search).ToAsyncEnumerable().FirstOrDefault();
					break;

				default:
					throw new NotSupportedException("bad kind");
			}

			//await logger_tsk;
			return new JsonResult(await found);
		}
	}
}

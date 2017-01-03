using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	public class HashesController : Controller
	{
		private static HashesInfo _hashesInfo = null;
		private static object _locker = new object();
		private BloggingContext _dbaseContext;
		private ILogger<HashesController> _logger;

		public HashesController(BloggingContext context, ILogger<HashesController> logger)
		{
			_dbaseContext = context;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var tsk = Task.Run(() =>
			{
				if (_hashesInfo == null)
				{
					lock (_locker)
					{
						HashesInfo hi = new HashesInfo();

						var alphabet = (from h in _dbaseContext.Hashes
										select h.SourceKey.First()
										).Distinct()
										.OrderBy(o => o)/*
										.SelectMany(m => m)*/;
						var count = _dbaseContext.Hashes.Count();
						var key_length = _dbaseContext.Hashes.Max(x => x.SourceKey.Length);

						hi.Count = count;
						hi.KeyLength = key_length;
						hi.Alphabet = string.Concat(alphabet);

						_hashesInfo = hi;
					}
				}
				return _hashesInfo;
			});

			return View(await tsk);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
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

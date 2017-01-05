using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	public class HashesController : Controller
	{
		//TODO: test knockout.js, test ApplicationInsights, test angular.js(2)

		private static HashesInfo _hashesInfo = new HashesInfo { IsReady = false };
		private static readonly object _locker = new object();
		private readonly IConfiguration _configuration;
		private readonly BloggingContext _dbaseContext;
		private readonly ILogger<HashesController> _logger;

		public HashesController(BloggingContext context, ILogger<HashesController> logger, IConfiguration configuration)
		{
			_dbaseContext = context;
			_logger = logger;
			_configuration = configuration;
		}

		public /*async Task<*/IActionResult/*>*/ Index()
		{
			if (!_hashesInfo.IsReady)
			{
				Task.Factory.StartNew((conn_str) =>
				{
					lock (_locker)
					{
						if (_hashesInfo.IsReady) return _hashesInfo;

						using (var db = new BloggingContext((string)conn_str))
						{
							var alphabet = (from h in db.Hashes
											select h.Key.First()
											).Distinct()
											.OrderBy(o => o)/*
										.SelectMany(m => m)*/;
							var count = db.Hashes.Count();
							var key_length = db.Hashes.Max(x => x.Key.Length);

							HashesInfo hi = new HashesInfo
							{
								Count = count,
								KeyLength = key_length,
								Alphabet = string.Concat(alphabet),
								IsReady = true,
							};
							_hashesInfo = hi;
						}
					}
					return _hashesInfo;
				}, _configuration.GetConnectionString("MySQL"));
			}

			return View(_hashesInfo);
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

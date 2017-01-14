using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EFGetStarted.AspNetCore.ExistingDb.Controllers
{
	/// <summary>
	/// TODO: test knockout.js, test ApplicationInsights
	/// </summary>
	public class HashesController : Controller
	{
		private static HashesInfo _hashesInfo;
		private static readonly object _locker = new object();

		private readonly IConfiguration _configuration;
		private readonly BloggingContext _dbaseContext;
		private readonly ILogger<HashesController> _logger;

		public HashesController(BloggingContext context, ILogger<HashesController> logger, IConfiguration configuration)
		{
			_dbaseContext = context;
			_logger = logger;
			_configuration = configuration;

			ViewBag.Info = _hashesInfo;
		}

		public /*async Task<*/IActionResult/*>*/ Index()
		{
			if (_hashesInfo == null || !_hashesInfo.IsCalculating)
			{
				Task.Factory.StartNew((conf) =>
				{
					_logger.LogInformation(0, $"###Starting calculation thread");

					lock (_locker)
					{
						_logger.LogInformation(0, $"###Starting calculation of initial Hash parameters");

						if (_hashesInfo != null)
						{
							_logger.LogInformation(0, $"###Leaving calculation of initial Hash parameters; already present");
							return _hashesInfo;
						}

						_hashesInfo = new HashesInfo { IsCalculating = true };
						//var bc = new DbContextOptionsBuilder<BloggingContext>();
						//Startup.ConfigureDBKind(bc, (IConfiguration)conf);

						//using (var db = new BloggingContext(bc.Options))
						{
						//	var alphabet = (from h in db.Hashes
						//					select h.Key.First()
						//					).Distinct()
						//					.OrderBy(o => o);
						//	var count = db.Hashes.Count();
						//	var key_length = db.Hashes.Max(x => x.Key.Length);

							_hashesInfo.Count = /*count*/123456789;
							_hashesInfo.KeyLength = /*key_length*/5;
							_hashesInfo.Alphabet = /*string.Concat(alphabet)*/"qwertyuiop";
							_hashesInfo.IsCalculating = false;

							_logger.LogInformation(0, $"###Calculation of initial Hash parameters ended");
						}
						ViewBag.Info = _hashesInfo;
					}
					return _hashesInfo;
				}, _configuration);
			}

			_logger.LogInformation(0, $"###Returning {nameof(_hashesInfo)}.{nameof(_hashesInfo.IsCalculating)} = {(_hashesInfo != null ? _hashesInfo.IsCalculating.ToString() : "null")}");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> Search(Hashes h, bool ajax)
		{
			if (!ModelState.IsValid)
			{
				return new JsonResult(null);
			}

			string search = h.Search;
			string shaKind = h.Kind.ToString();

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

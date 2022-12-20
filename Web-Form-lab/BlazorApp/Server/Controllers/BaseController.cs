using Microsoft.AspNetCore.Mvc;
using Whosales.Web.Services;

namespace Whosales.Web.Controllers
{
	[Route("api/")]
	public abstract class BaseController<TService, TEntity> : Controller
		where TService : BaseTableService<TEntity>
		where TEntity : class
	{
		private TService _service;
		protected TService Service => _service;
		public BaseController(TService service)
		{
			_service = service;
		}

		protected string GetValueFromCookie(HttpRequest request, string cookieName, string queryName, string defaultValue = "")
		{
			if (request.Query[queryName].Count() > 0)
			{
				return request.Query[queryName][0];
			}
			else if (request.Cookies[cookieName] != null)
			{
				return request.Cookies[cookieName];
			}
			else
			{
				return defaultValue;
			}
		}
	}
}

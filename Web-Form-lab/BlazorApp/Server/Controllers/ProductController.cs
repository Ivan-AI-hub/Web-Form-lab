using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whosales.Domain;
using Whosales.Web.Services;

namespace Whosales.Web.Controllers
{
	[Route("/api/products/")]
	public class ProductController : BaseController<ProductService, Product>
	{
		private static string _previousUrl = "";
		public ProductController(ProductService service) : base(service)
		{
		}

		#region Create
		[HttpPost]
		public IActionResult Post(Product product)
		{
			if (product == null)
			{
				return BadRequest();
			}

			Service.Add(product);
			return Ok(product);
		}
		#endregion

		#region Update
		[HttpPut]
		public ActionResult Put(int id, Product product)
		{
			if (product == null)
			{
				return BadRequest();
			}

			Service.Update(id, product);
			return Redirect(_previousUrl);
		}
		#endregion

		#region Delete
		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Service.Delete(id);
			return Ok();
		}
		#endregion

		#region Read
		[HttpGet]
		public async Task<IEnumerable<Product>> Get()
		{
			var products = await Service.GetAll();

			return products;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await Service.GetById(id);
			if (product == null)
				return NotFound();

			return new ObjectResult(product);
		}
		#endregion
	}
}

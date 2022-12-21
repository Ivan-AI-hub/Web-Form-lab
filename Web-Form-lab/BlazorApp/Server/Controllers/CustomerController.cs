using Microsoft.AspNetCore.Mvc;
using Whosales.Domain;
using Whosales.Web.Services;

namespace Whosales.Web.Controllers
{
	[Route("/api/customers")]
	public class CustomerController : BaseController<CustomerService, Customer>
	{
		private static string _previousUrl = "";
		
		public CustomerController(CustomerService service) : base(service)
		{

		}
		#region Create
		[HttpPost]
		public IActionResult Post([FromBody]Customer customer)
		{
			if(customer == null)
				return BadRequest();

			Service.Add(customer);
			return Ok(customer);
		}
		#endregion

		#region Update
		[HttpPut("{id}")]
		public ActionResult Put([FromRoute]int id, [FromBody]Customer customer)
		{
			if (customer == null)
				return BadRequest();

			Service.Update(id, customer);
			return Ok(customer);
		}
		#endregion

		#region Delete
		[HttpDelete("{id}")]
		public ActionResult Delete([FromRoute]int id)
		{
			Service.Delete(id);
			return Ok();
		}
		#endregion

		#region Read
		[HttpGet]
		public async Task<IEnumerable<Customer>> GetAll()
		{
			var customers = await Service.GetAll();
			return customers;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			var customer = await Service.GetById(id);
			if (customer == null)
				return NotFound();
			return new ObjectResult(customer);
		}
		#endregion
	}
}

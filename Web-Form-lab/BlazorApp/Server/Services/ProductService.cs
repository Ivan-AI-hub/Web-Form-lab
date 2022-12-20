using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Whosales.Application.Comands.Products;
using Whosales.Application.Queries.Products;
using Whosales.Domain;

namespace Whosales.Web.Services
{
	public class ProductService : BaseTableService<Product>
	{
		public ProductService(IMemoryCache cache, IMediator mediator) : base(cache, mediator)
		{
		}

		public override async Task<IEnumerable<Product>> GetPage(int pageNumber, Func<Product, bool>? whereRule = null, string sortRule = "")
		{
			IEnumerable<Product> products;
			Func<Product, dynamic> orderRule;
			TranslateSortRule(sortRule, out orderRule);

			SetPageCount(whereRule).Wait();
			products = await Mediator.Send(new GetProductsQueryPage(PageSystemModel.PageSize, pageNumber, orderRule, whereRule));
			products = products.ToList();

			return products ?? new List<Product>();
		}

		public async override Task<IEnumerable<Product>> GetAll()
		{
			IEnumerable<Product> items;
			string cashKey = "Items";
			if (!Cache.TryGetValue(cashKey, out items))
			{
				items = await Mediator.Send(new GetProductsQuery());
				items = items.ToList();
				Cache.Set(cashKey, items, TimeSpan.FromSeconds(CacheTime));
			}
			return items;
		}

		public override async Task<Product?> GetById(int id)
		{
			Product? product = await Mediator.Send(new GetProductById(id));
			return product;
		}

		public async override void Add(Product value)
		{
			await Mediator.Send(new AddProduct(value));
			CacheClear();
		}

		public async override void Update(int id, Product newValue)
		{
			await Mediator.Send(new UpdateProduct(id, newValue));
			CacheClear();
		}

		public async override void Delete(int id)
		{
			await Mediator.Send(new DeleteProduct(id));
			CacheClear();
		}

		private void TranslateSortRule(string sortRule, out Func<Product, dynamic> orderRule)
		{
			switch (sortRule)
			{
				case "Name":
					orderRule = x => x.Name;
					break;
				case "ManufacturerName":
					orderRule = x => x.Manufacturer.Name;
					break;
				case "Type":
					orderRule = x => x.Type.Name;
					break;
				case "StorageConditions":
					orderRule = x => x.StorageConditions;
					break;
				case "Package":
					orderRule = x => x.Package;
					break;
				case "StorageLife":
					orderRule = x => x.StorageLife;
					break;
				default:
					orderRule = x => x.Name;
					break;
			}
		}

		private async Task SetPageCount(Func<Product, bool>? whereRule = null)
		{
			PageSystemModel.PageCount = 100;
		}
	}
}

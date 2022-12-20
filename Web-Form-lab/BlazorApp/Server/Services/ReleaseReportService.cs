using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Whosales.Application.Comands.ReleaseReports;
using Whosales.Application.Queries.ReleaseReports;
using Whosales.Domain;

namespace Whosales.Web.Services
{
	public class ReleaseReportService : BaseTableService<ReleaseReport>
	{
		public ReleaseReportService(IMemoryCache cache, IMediator mediator) : base(cache, mediator)
		{
		}

		public async override void Add(ReleaseReport value)
		{
			await Mediator.Send(new AddReleaseReport(value));
			CacheClear();
		}

		public async override void Delete(int id)
		{
			await Mediator.Send(new DeleteReleaseReport(id));
			CacheClear();
		}

		public override async Task<ReleaseReport?> GetById(int id)
		{
			ReleaseReport? ReleaseReport = await Mediator.Send(new GetReleaseReportById(id));
			return ReleaseReport;
		}

		public override async Task<IEnumerable<ReleaseReport>> GetPage(int pageNumber, Func<ReleaseReport, bool>? whereRule = null, string sortRule = "")
		{
			IEnumerable<ReleaseReport> ReleaseReports;
			Func<ReleaseReport, dynamic> orderRule;
			TranslateSortRule(sortRule, out orderRule);

			SetPageCount(whereRule).Wait();
			ReleaseReports = await Mediator.Send(new GetReleaseReportsQueryPage(PageSystemModel.PageSize, pageNumber, orderRule, whereRule));
			ReleaseReports = ReleaseReports.ToList();

			return ReleaseReports ?? new List<ReleaseReport>();
		}

		public async override Task<IEnumerable<ReleaseReport>> GetAll()
		{
			IEnumerable<ReleaseReport> items;

			items = await Mediator.Send(new GetReleaseReportsQuery());
			items = items.ToList();
			return items;
		}

		public async override void Update(int id, ReleaseReport newValue)
		{
			await Mediator.Send(new UpdateReleaseReport(id, newValue));
			CacheClear();
		}

		private void TranslateSortRule(string sortRule, out Func<ReleaseReport, dynamic> orderRule)
		{
			switch (sortRule)
			{
				case "Volume":
					orderRule = x => x.Volume;
					break;
				case "Cost":
					orderRule = x => x.Cost;
					break;
				case "ReciveDate":
					orderRule = x => x.ReciveDate;
					break;
				case "OrderDate":
					orderRule = x => x.OrderDate;
					break;
				case "ReleaseDate":
					orderRule = x => x.ReleaseDate;
					break;
				case "EmployerName":
					orderRule = x => x.Employer.Name;
					break;
				case "CustomerName":
					orderRule = x => x.Customer.Name;
					break;
				case "ProductName":
					orderRule = x => x.Product.Name;
					break;
				case "StorageName":
					orderRule = x => x.Storage.Name;
					break;
				default:
					orderRule = x => x.Volume;
					break;
			}
		}

		private async Task SetPageCount(Func<ReleaseReport, bool>? whereRule = null)
		{
			PageSystemModel.PageCount = (await Mediator.Send(new GetReleaseReportsCount(whereRule)) / PageSystemModel.PageSize + 1);
		}
	}
}

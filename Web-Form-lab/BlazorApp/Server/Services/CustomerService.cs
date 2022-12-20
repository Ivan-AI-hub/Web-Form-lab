using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Whosales.Application.Comands.Customers;
using Whosales.Application.Queries.Customers;
using Whosales.Domain;

namespace Whosales.Web.Services
{
    public class CustomerService : BaseTableService<Customer>
    {
        public CustomerService(IMemoryCache cache, IMediator mediator) : base(cache, mediator)
        {
        }

        public async override void Add(Customer value)
        {
            await Mediator.Send(new AddCustomer(value));
            CacheClear();
        }

        public async override void Delete(int id)
        {
            await Mediator.Send(new DeleteCustomer(id));
            CacheClear();
        }

        public override async Task<Customer?> GetById(int id)
        {
            Customer? customer = await Mediator.Send(new GetCustomerById(id));
            return customer;
        }

        public async override Task<IEnumerable<Customer>> GetPage(int pageNumber, Func<Customer, bool>? whereRule = null, string sortRule = "")
        {
            IEnumerable<Customer> customers;
            Func<Customer, dynamic> orderRule;
            TranslateSortRule(sortRule, out orderRule);
            SetPageCount(whereRule).Wait();
            customers = await Mediator.Send(new GetCustomersQueryPage(PageSystemModel.PageSize, pageNumber, orderRule, whereRule));
            customers = customers.ToList();

            return customers ?? new List<Customer>();
        }

        public async override Task<IEnumerable<Customer>> GetAll()
        {
            IEnumerable<Customer> customers;
            string cashKey = "Items";
            if (!Cache.TryGetValue(cashKey, out customers))
            {
                customers = await Mediator.Send(new GetCustomersQuery());
                customers = customers.ToList();
                Cache.Set(cashKey, customers, TimeSpan.FromSeconds(CacheTime));
            }
            return customers;
        }

        public async override void Update(int id, Customer newValue)
        {
            await Mediator.Send(new UpdateCustomer(id, newValue));
            CacheClear();
        }

        private void TranslateSortRule(string sortRule, out Func<Customer, dynamic> orderRule)
        {
            if (sortRule == "Name")
            {
                orderRule = x => x.Name;
            }
            else if (sortRule == "Address")
            {
                orderRule = x => x.Address;
            }
            else if (sortRule == "TelephoneNumber")
            {
                orderRule = x => x.TelephoneNumber;
            }
            else if (sortRule == "OrdersCount")
            {
                orderRule = x => x.ReleaseReports.Count;
            }
            else
            {
                orderRule = x => x.Name;
            }
        }

        private async Task SetPageCount(Func<Customer, bool>? whereRule = null)
        {
            PageSystemModel.PageCount = (await Mediator.Send(new GetCustomersCount(whereRule)) / PageSystemModel.PageSize + 1);
        }
    }
}

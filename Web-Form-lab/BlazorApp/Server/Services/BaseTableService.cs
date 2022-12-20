using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Whosales.Web.Models;

namespace Whosales.Web.Services
{
    public abstract class BaseTableService<T> where T : class
    {
        public PageSystemModel PageSystemModel;

        private IMemoryCache _cache;
        private IMediator _mediator;

        protected IMemoryCache Cache => _cache;
        protected IMediator Mediator => _mediator;

        protected int CacheTime = 240;

        public BaseTableService(IMemoryCache cache, IMediator mediator)
        {
            _cache = cache;
            _mediator = mediator;
            PageSystemModel = new PageSystemModel();
        }

        public abstract Task<IEnumerable<T>> GetPage(int pageNumber, Func<T, bool>? whereRule = null, string sortRule = "");
        public abstract Task<IEnumerable<T>> GetAll();
        public abstract Task<T?> GetById(int id);
        public abstract void Add(T value);
        public abstract void Update(int id, T newValue);
        public abstract void Delete(int id);

        protected void CacheClear()
        {
            MethodInfo clearMethod = Cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            if (clearMethod != null)
            {
                clearMethod.Invoke(Cache, null);
                return;
            }
            else
            {
                PropertyInfo prop = Cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                if (prop != null)
                {
                    object innerCache = prop.GetValue(Cache);
                    if (innerCache != null)
                    {
                        clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                        if (clearMethod != null)
                        {
                            clearMethod.Invoke(innerCache, null);
                            return;
                        }
                    }
                }
            }
        }

    }
}

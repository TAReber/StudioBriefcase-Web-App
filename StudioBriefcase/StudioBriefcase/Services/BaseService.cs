using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using StudioBriefcase.Helpers;
using StudioBriefcase.Models;
//using StudioBriefcase.Helpers;

namespace StudioBriefcase.Services
{
    public class BaseService : IBaseService
    {
        protected readonly MySqlConnection _connection;
        protected readonly IMemoryCache _cache;
        public BaseService(IMemoryCache cache, MySqlConnection connection)
        {
            _cache = cache;
            _connection = connection;
            Console.WriteLine("BaseService Constructor");
        }

        public Task<SelectorListModel> GetCategoryListAsync()
        {
            //MySqlCommand command = new QueryHelper().Select("id, category_name").From("categories").Order("category_name").Build(_connection);
            MySqlCommand command = new MySqlCommand("SELECT id, category_name from categories ORDER BY category_name ASC;", _connection);




            throw new NotImplementedException();
        }
    }
}

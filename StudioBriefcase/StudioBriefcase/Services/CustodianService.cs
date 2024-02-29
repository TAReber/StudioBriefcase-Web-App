using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;

namespace StudioBriefcase.Services
{
    public class CustodianService : BaseService, ICustodianService
    {
        public CustodianService(IMemoryCache cache, MySqlConnection connection) : base(cache, connection)
        {
            Console.WriteLine("CustodianService Constructor");
        }

        public async Task SetLibraryQuickLinksAsync(string libraryName, string jsonString)
        {

            await _connection.OpenAsync(); //CONNECTION TEST1
            var query = new MySqlCommand($"UPDATE library_links SET links = @jsonString WHERE library_id = (SELECT id FROM libraries WHERE library_name = @libraryName);", _connection);
            query.Parameters.AddWithValue("@jsonString", jsonString);
            query.Parameters.AddWithValue("@libraryName", libraryName);

            await query.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            _cache.Remove(libraryName);
        }   


    public async Task AddTag(string tagname)
    {
        //TODO CHECK if Tag Already Exists.
        //await _connection.OpenAsync();
        var query = new MySqlCommand($"INSERT INTO tags (tag) VALUES (@name_of_tag);", _connection);
        query.Parameters.AddWithValue("@name_of_tag", tagname);

        await query.ExecuteNonQueryAsync();
        //await _connection.CloseAsync();
        _cache.Remove("tags");
    }
    }
}

using MySqlConnector;
using StudioBriefcase.Models;
using System.Data;


namespace StudioBriefcase.Services
{
    public class UserService : IUserService
    {
        private readonly MySqlConnection _connection;
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger, MySqlConnection connection)
        {
            _connection = connection;
            _logger = logger;
        }


        public async Task AddUserAsync(UserModel user)
        {
            try
            {
                await _connection.OpenAsync();
                var query = new MySqlCommand($"INSERT INTO gituser (gituser_id, gituser_name, gituser_site, gituser_pic, class_id, privilege_id) VALUES (@id, @name, @profile, @avatar, @class, @privilege);", _connection);
                query.Parameters.AddWithValue("@id", user.Id);
                query.Parameters.AddWithValue("@name", user.Name);
                query.Parameters.AddWithValue("@profile", user.profile_url);
                query.Parameters.AddWithValue("@avatar", user.avatar_url);
                query.Parameters.AddWithValue("@class", user.userclass);
                query.Parameters.AddWithValue("@privilege", user.userprivilege);

                var affectedRows = await query.ExecuteNonQueryAsync();
                if (affectedRows == 0)
                {
                    _logger.LogError($"{user.Name} Failed to Add User to Database");
                }
                else
                {
                    _logger.LogInformation($"{user.Name} Added to Database");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            finally
            {
                await _connection.CloseAsync();
            }

        }

        public async Task<string> GetUserClass(uint user_id)
        {
            await _connection.OpenAsync();
            var query = new MySqlCommand($"SELECT class FROM user_classes WHERE id = (SELECT class_id FROM gituser WHERE gituser_id = @id);", _connection);
            query.Parameters.AddWithValue("@id", user_id);

            using(var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var role = reader.GetString(0);
                    await _connection.CloseAsync();
                    return role;
                }
            }
            await _connection.CloseAsync();
            return "Error";
        }

        public async  Task<string> GetUserPrivilege(uint user_id)
        {
            await _connection.OpenAsync();
            var query = new MySqlCommand($"SELECT privilege FROM user_privileges WHERE id = (SELECT privilege_id FROM gituser WHERE gituser_id = @id);", _connection);
            query.Parameters.AddWithValue("@id", user_id);

            using (var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var role = reader.GetString(0);
                    await _connection.CloseAsync();
                    return role;
                }
            }
            await _connection.CloseAsync();
            return "Error";
        }
        
        public async Task SetUserClass(uint user_id, string userclass)
        {
            
            //_logger.LogError($"Setting {user_id} user Role to {userclass}");
            await _connection.OpenAsync();
            var query = new MySqlCommand($"UPDATE gituser SET class_id = (SELECT id FROM user_classes WHERE class = @class) WHERE gituser_id = @id;", _connection);
            query.Parameters.AddWithValue("@class", userclass);
            query.Parameters.AddWithValue("@id", user_id);


            try
            {
                var reader = await query.ExecuteNonQueryAsync();
                Console.WriteLine($"UserClass Updated, {user_id}, {userclass}");
                
            }
            catch (Exception e)
            {
                if (_connection.State == ConnectionState.Open)
                    Console.WriteLine("Connection Open");
                _logger.LogError($"SetUserClass in UserService {e.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
       
        }

        public async Task<bool> UserExistsAsync(uint user_id)
        {
            bool exists = false;

            await _connection.OpenAsync();
            var query = new MySqlCommand($"SELECT count(1) from gituser where gituser_id = @id;", _connection);
            query.Parameters.AddWithValue("@id", user_id);

            using (var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var count = reader.GetInt32(0);
                    if (count > 0)
                    {
                        exists = true;
                        //_logger.LogError("User Already Exists in Database");
                    }
                }
            }
            await _connection.CloseAsync();
            return exists;
        }
    }
}

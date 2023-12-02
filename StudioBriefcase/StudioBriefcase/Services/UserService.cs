using MySqlConnector;
using StudioBriefcase.Models;


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
                var query = new MySqlCommand($"INSERT INTO gituser (gituser_id, gituser_name, gituser_site, gituser_pic, role_id) VALUES (@id, @name, @profile, @avatar, @role);", _connection);
                query.Parameters.AddWithValue("@id", user.Id);
                query.Parameters.AddWithValue("@name", user.Name);
                query.Parameters.AddWithValue("@profile", user.profile_url);
                query.Parameters.AddWithValue("@avatar", user.avatar_url);
                query.Parameters.AddWithValue("@role", user.role);

                var affectedRows = await query.ExecuteNonQueryAsync();
                if (affectedRows == 0)
                {
                    _logger.LogError("Failed to Add User to Database");
                }
                else
                {
                    _logger.LogInformation("User Added to Database");
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

        public async Task<string> GetUserRole(uint user_id)
        {
            await _connection.OpenAsync();
            var query = new MySqlCommand($"SELECT userRole FROM roles WHERE id = (SELECT role_id FROM gituser WHERE gituser_id = @id);", _connection);
            query.Parameters.AddWithValue("@id", user_id);

            using(var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var role = reader.GetString(0);
                    
                    return role;
                }
            }
            await _connection.CloseAsync();
            return "Error";
        }
        
        public Task SetUserRole(string role)
        {
            _logger.LogError($"Setting user Role to {role}");
            throw new NotImplementedException();
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

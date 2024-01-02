using StudioBriefcase.Models;
using MySqlConnector;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

namespace StudioBriefcase.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly MySqlConnection _connection;
        private readonly IMemoryCache _cache;

        public LibraryService(IMemoryCache cache, MySqlConnection connection)
        {
            _connection = connection;
            _cache = cache;
        }


        public async Task<List<LibraryLinksModel>> GetLibraryLinksAsync(string libraryName)
        {
            //I'm going to try to cache the list thats created to reduce the database traffic.
            if(_cache.TryGetValue(libraryName, out List<LibraryLinksModel>? cachedlinks))
            {
                if(cachedlinks != null)
                {
                    return cachedlinks;
                }            
            }

            //If CachedData is expired or failed to detect cached list, retrieve from database.
            await _connection.OpenAsync();

            var query = new MySqlCommand($"SELECT links from library_links WHERE library_id = (SELECT id from libraries WHERE library_name = @libraryName);", _connection);
            query.Parameters.AddWithValue("@libraryName", libraryName);


            using (var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var json = reader.GetString(0);
                    var quicklinks = JsonSerializer.Deserialize<List<LibraryLinksModel>>(json);
                    if (quicklinks != null && quicklinks.Count != 0)
                    {
                        _cache.Set(libraryName, quicklinks, TimeSpan.FromMinutes(30));
                        return quicklinks;
                    }
                }
            }
            return Error_GetLibraryLinksAsync();
        }

        public async Task<List<SubjectModel>> GetSubjectListAsync(string path)
        {
            string[] folders = path.Split('\\');
            string library = folders[folders.Length - 2];
            string category = folders[folders.Length - 3];

            await _connection.OpenAsync();
            var query = new MySqlCommand($"SELECT subjects.subject_name, topics.topic_name FROM subjects join topics on subjects.id = topics.subject_id where library_id =  (SELECT id from libraries where library_name = @variable) ORDER BY subjects.priority, topics.priority;", _connection);

            query.Parameters.AddWithValue("@variable", library);

            using (var reader = query.ExecuteReader())
            {
                List<SubjectModel> SubjectModel_List = new List<SubjectModel>();
                string subject_temp = string.Empty;
                int iterator = -1;
                while (await reader.ReadAsync())
                {
                     
                    string subjectName = reader.GetString(0);

                    if (subjectName != subject_temp)
                    {
                        iterator++;
                        subject_temp = subjectName;

                        SubjectModel_List.Add(
                            new SubjectModel()
                            {
                                Name = subjectName,
                                Topics =
                                {
                                    new TopicModel()
                                    {
                                        Name = reader.GetString(1),
                                        PathUrl = $"/Library/{category}/{library}/{reader.GetString(0)}/{reader.GetString(1)}"
                                    }
                                }

                            });
                    }
                    else
                    {
                        SubjectModel_List[iterator].Topics.Add(new TopicModel
                        {
                            Name = reader.GetString(1),
                            PathUrl = $"/Library/{category}/{library}/{reader.GetString(0)}/{reader.GetString(1)}"

                        });
                    }
                    

                }
                if (SubjectModel_List != null && SubjectModel_List.Count > 0)
                {
                    string cacheKey = $"{library}_subjects";
                    _cache.Set(cacheKey, SubjectModel_List, TimeSpan.FromMinutes(30));
                    return SubjectModel_List;
                }


            }


            return Error_GetSubjectListAsync();
        }

        public async Task SetLibraryQuickLinksAsync(string libraryName, string jsonString)
        {
            await _connection.OpenAsync();
            var query = new MySqlCommand($"UPDATE library_links SET links = @jsonString WHERE library_id = (SELECT id FROM libraries WHERE library_name = @libraryName);", _connection);
            query.Parameters.AddWithValue("@jsonString", jsonString);
            query.Parameters.AddWithValue("@libraryName", libraryName);

            await query.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            _cache.Remove(libraryName);
        }


        private List<LibraryLinksModel> Error_GetLibraryLinksAsync()
        {
            return new List<LibraryLinksModel>
            {
                new LibraryLinksModel
                {
                    SiteUrl = "#",
                    ImgSource = "",
                    ShorthandDesc = "ERROR"
                },

            };

        }
        private List<SubjectModel> Error_GetSubjectListAsync()
        {
            return new List<SubjectModel>
            {

                new SubjectModel { Name = "No Subjects Found",
                Topics = new List<TopicModel>
                    {
                        new TopicModel
                        {
                            Name = "No Topics Found",
                            PathUrl= "#",
                        }
                    }
                }
            };
        }
    }

}


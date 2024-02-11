using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using StudioBriefcase.Models;
using System.Text.Json;

namespace StudioBriefcase.Services
{
    public class PageService : BaseService, IPageService
    {
        //PostTypeService moved to new class to manage posts that inherits from Library Service
        public PageService(IMemoryCache cache, MySqlConnection connection) : base(cache, connection)
        {
           Console.WriteLine("PageService Constructor");
           
            _connection.Open();
        }

        public async Task<List<LibraryQuickLinksModel>> GetLibraryQuickLinksAsync(string libraryName)
        {
            //I'm going to try to cache the list thats created to reduce the database traffic.
            if (_cache.TryGetValue(libraryName, out List<LibraryQuickLinksModel>? cachedlinks))
            {
                if (cachedlinks != null)
                {
                    return cachedlinks;
                }
            }

            //If CachedData is expired or failed to detect cached list, retrieve from database.

            //await _connection.OpenAsync(); //CONNECTION TEST1
            
            var query = new MySqlCommand($"SELECT links from library_links WHERE library_id = (SELECT id from libraries WHERE library_name = @libraryName);", _connection);
            query.Parameters.AddWithValue("@libraryName", libraryName);


            using (var reader = query.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    var json = reader.GetString(0);
                    var quicklinks = JsonSerializer.Deserialize<List<LibraryQuickLinksModel>>(json);
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

            //await _connection.OpenAsync(); //CONNECTION TEST1
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
        public async Task<LibraryTagsListModel> GetLibraryTagsAsync()
        {
            //_cache.Remove(_tagsCacheKey);

            if (_cache.TryGetValue("tags", out LibraryTagsListModel? cachedtags))
            {
                if (cachedtags != null)
                {
                    return cachedtags;
                }
            }


            LibraryTagsListModel tagLists = new LibraryTagsListModel();

            //await _connection.OpenAsync(); //CONNECTION TEST1
            var query = new MySqlCommand("SELECT * FROM tags ORDER BY tag;", _connection);
            using (var reader = query.ExecuteReader())
            {

                while (await reader.ReadAsync())
                {
                    uint tagid = reader.GetFieldValue<uint>(0);
                    string name = reader.GetFieldValue<string>(1);
                    int area = reader.GetFieldValue<int>(2);

                    LibraryTagsModel tags = new LibraryTagsModel()
                    {
                        id = tagid,
                        tagName = name
                    };
                    if (area == 1)
                        tagLists.Tags_normal.Add(tags);
                    else if (area == 2)
                        tagLists.Tags_IDE.Add(tags);
                    else if (area == 3)
                        tagLists.Tags_OS.Add(tags);

                    _cache.Set("tags", tagLists, TimeSpan.FromMinutes(300));
                }
            }
            //await _connection.CloseAsync();
            return tagLists;
        }





        private List<LibraryQuickLinksModel> Error_GetLibraryLinksAsync()
        {
            return new List<LibraryQuickLinksModel>
            {
                new LibraryQuickLinksModel
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

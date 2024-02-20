using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using StudioBriefcase.Helpers;
using StudioBriefcase.Models;
using System.Text;
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
            
        }

        public Task<LibraryMapIDsModel> BuildMapFromTopicID(uint topicID)
        {
            
            LibraryMapIDsModel map = new LibraryMapIDsModel();

            MySqlCommand command = new QueryHelper()
                .SelectMapID(topicID)
                .JoinMap()
                .Build(_connection);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    
                    map.LibraryID = reader.GetUInt32(0);
                    map.CategoryID = reader.GetUInt32(1);
                    map.SubjectID = reader.GetUInt32(2);

                    map.TopicID = topicID;
                }
            }

            return Task.FromResult(map);
        }

        public async Task<SelectorListModel> GetCategoryListAsync()
        {            
            SelectorListModel categoryList = new SelectorListModel();
            MySqlCommand command = new QueryHelper().Select("id, category_name").From("categories").Order("category_name").Build(_connection);

            using(var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    categoryList.list.Add(new ID_String_Pair_Model
                    {
                        id = reader.GetUInt32(0),
                        text = reader.GetString(1)
                    });
                }
            }

            return categoryList;
        }

        public async Task<SelectorListModel> GetLibraryListAsync(uint categoryID)
        {
            SelectorListModel libraryList = new SelectorListModel();
            MySqlCommand command = new QueryHelper().Select("id, library_name").From("libraries").Where("id", categoryID).Order("library_name").Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    libraryList.list.Add(new ID_String_Pair_Model
                    {
                        id = reader.GetUInt32(0),
                        text = reader.GetString(1)
                    });
                }
            }

            return libraryList;

        }

        public async Task<SelectorListModel> GetSubjectListAsync(uint libraryID)
        {
            SelectorListModel subjectList = new SelectorListModel();
            MySqlCommand command = new QueryHelper().Select("id, subject_name").From("subjects").Where("library_id", libraryID).Order("subject_name").Build(_connection);
            using(var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    subjectList.list.Add(new ID_String_Pair_Model
                    {
                        id = reader.GetUInt32(0),
                        text = reader.GetString(1)
                    });
                }
            }
            return subjectList;
        }

        public async Task<SelectorListModel> GetTopicListAsync(uint subjectID)
        {
            SelectorListModel topicList = new SelectorListModel();
            MySqlCommand command = new QueryHelper().Select("id, topic_name")
                .From("topics")
                .Where("subject_id", subjectID)
                .Order("topic_name")
                .Build(_connection);
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    topicList.list.Add(new ID_String_Pair_Model
                    {
                        id = reader.GetUInt32(0),
                        text = reader.GetString(1)
                    });
                }
            }
            return topicList;

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

                    ID_String_Pair_Model tags = new ID_String_Pair_Model()
                    {
                        id = tagid,
                        text = name
                    };
                    if (area == 1)
                        tagLists.Tags_OS.list.Add(tags);                      
                    else if (area == 2)
                        tagLists.Tags_IDE.list.Add(tags);
                    else if (area == 3)
                        tagLists.Tags_normal.list.Add(tags);


                    _cache.Set("tags", tagLists, TimeSpan.FromMinutes(300));
                }
            }
            //await _connection.CloseAsync();
            return tagLists;
        }

        public Task<string> GetDirectyPathMap(uint TopicID)
        {
            string path = string.Empty;
            MySqlCommand command = new QueryHelper().SelectMapName(TopicID).JoinMap().Build(_connection);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    StringBuilder pathBuilder = new StringBuilder(reader.GetString(0));
                    pathBuilder.Append($"/{reader.GetString(1)}");
                    pathBuilder.Append($"/{reader.GetString(2)}");
                    pathBuilder.Append($"/{reader.GetString(3)}");
                    path = pathBuilder.ToString();
                }
            }

            return Task.FromResult(path);
        }
    }
}

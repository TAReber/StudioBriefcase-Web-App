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

        public async Task<SelectorListModel> GetCategoryListAsync(uint language)
        {
            SelectorListModel categoryList = new SelectorListModel();
            MySqlCommand command;
            if (language == 0)
            {
                command = new QueryHelper().Select("id, category_name").From("categories").Order("category_name").Build(_connection);
            }
            else
            {
                command = new QueryHelper().Select("c.id, a.alias_name").From("Categories c")
                    .Join("Categories_languages a ON a.map_id = c.id")
                    .Where("a.language_id", language).Build(_connection);
            }

            using (var reader = await command.ExecuteReaderAsync())
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

        public async Task<SelectorListModel> GetLibraryListAsync(uint categoryID, uint languageID)
        {
            SelectorListModel libraryList = new SelectorListModel();
            
            MySqlCommand command;
            if (languageID == 0)
            {
                command = new QueryHelper().Select("id, library_name").From("libraries").Where("id", categoryID).Order("library_name").Build(_connection);
            }
            else
            {
                command = new QueryHelper().Select("l.id, a.alias_name").From("libraries l")
                    .Join("libraries_languages a ON a.map_id = l.id")
                    .Where("a.language_id", languageID).WhereAnd("l.category_id", categoryID).Build(_connection);
            }
             

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

        public async Task<SelectorListModel> GetSubjectListAsync(uint libraryID, uint languageID)
        {
            SelectorListModel subjectList = new SelectorListModel();

            MySqlCommand command;
            if (languageID == 0)
            {
                command = new QueryHelper().Select("id, subject_name").From("subjects").Where("library_id", libraryID).Order("subject_name").Build(_connection);
            }
            else
            {
                command = new QueryHelper().Select("s.id, a.alias_name").From("subjects s")
                    .Join("subjects_languages a ON a.map_id = s.id")
                    .Where("a.language_id", languageID).WhereAnd("s.library_id", libraryID).Build(_connection);
            }
            //MySqlCommand command = new QueryHelper().Select("id, subject_name").From("subjects").Where("library_id", libraryID).Order("subject_name").Build(_connection);
            using (var reader = await command.ExecuteReaderAsync())
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

        public async Task<SelectorListModel> GetTopicListAsync(uint subjectID, uint languageID)
        {
            SelectorListModel topicList = new SelectorListModel();

            MySqlCommand command;
            if (languageID == 0)
            {
                command = new QueryHelper().Select("id, topic_name")
                    .From("topics")
                    .Where("subject_id", subjectID)
                    .Order("topic_name")
                .Build(_connection);

            }
            else
            {
                command = new QueryHelper().Select("t.id, a.alias_name").From("topics t")
                    .Join("topics_languages a ON a.map_id = t.id")
                    .Where("a.language_id", languageID).WhereAnd("t.subject_id", subjectID).Build(_connection);
            }

            //MySqlCommand command = new QueryHelper().Select("id, topic_name")
            //    .From("topics")
            //    .Where("subject_id", subjectID)
            //    .Order("topic_name")
            //    .Build(_connection);
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


        public async Task<SelectorListModel> GetLanguages()
        {
            //_cache.Remove("languages");
            SelectorListModel languages;
            if (_cache.TryGetValue("languages", out SelectorListModel? cachedlist))
            {

                languages = cachedlist ?? new SelectorListModel();
            }
            else
            {

                languages = new SelectorListModel();
                try
                {
                    MySqlCommand command = new QueryHelper().Select("id, lang").From("languages").Order("id").Build(_connection);

                    //var reader = await command.ExecuteReaderAsync();
                    //while (await reader.ReadAsync())
                    //{
                    //    languages.list.Add(new ID_String_Pair_Model
                    //    {
                    //        id = reader.GetUInt32(0),
                    //        text = reader.GetString(1)
                    //    });
                    //}

                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            languages.list.Add(new ID_String_Pair_Model
                            {
                                id = reader.GetUInt32(0),
                                text = reader.GetString(1)

                            });
                        }

                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    _cache.Set("languages", languages, TimeSpan.FromMinutes(30));
                }

            }

            return languages;
        }

        public async Task<uint> LanguagesIDexists(string languageName)
        {

            MySqlCommand command = new QueryHelper().Select("id")
                .From("languages").Where("lang", languageName).Build(_connection);

            uint id = 0;
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    id = reader.GetFieldValue<uint>(0);
                }

            }


            return id;
        }
    }
}

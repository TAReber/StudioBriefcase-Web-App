using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using StudioBriefcase.Helpers;
using StudioBriefcase.Models;
using System.Text.Json;

namespace StudioBriefcase.Services
{
    public class PageService : BaseService, IPageService
    {
        //PostTypeService moved to new class to manage posts that inherits from Library Service
        public PageService(IMemoryCache cache, MySqlConnection connection) : base(cache, connection)
        {

            _connection.Open();

        }

        public async Task<List<Map_Alias_Pair_Model>> GetCategoryAliasIntros(uint languageID)
        {

            List<Map_Alias_Pair_Model> intros = new List<Map_Alias_Pair_Model>();
            MySqlCommand command = new QueryHelper().Select("c.id, c.category_name, a.alias_name, a.alias_description")
                .From("categories_languages a")
                .Join("categories c ON c.id = a.map_id")
                .Where("a.language_id", languageID)
                .Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    intros.Add(new Map_Alias_Pair_Model(
                        new ID_String_Pair_Model
                        {
                            id = reader.GetFieldValue<uint>(0),
                            text = reader.GetString(1)
                        },
                        new AliasNamesModel
                        {
                            name = reader.GetString(2),
                            description = reader.GetString(3)
                        }));
                }
            }

            return intros;
        }

        public async Task<List<Map_Alias_Pair_Model>> GetLibraryAliasIntros(uint CategoryID, uint languageID)
        {
            List<Map_Alias_Pair_Model> intros = new List<Map_Alias_Pair_Model>();
            MySqlCommand command = new QueryHelper().Select("l.id, l.library_name, a.alias_name, a.alias_description")
                .From("libraries_languages a")
                .Join("libraries l ON l.id = a.map_id")
                .Where("a.language_id", languageID).WhereAnd("l.category_id", CategoryID)
                .Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    intros.Add(new Map_Alias_Pair_Model(
                        new ID_String_Pair_Model
                        {
                            id = reader.GetFieldValue<uint>(0),
                            text = reader.GetString(1)
                        },
                        new AliasNamesModel
                        {
                            name = reader.GetString(2),
                            description = reader.GetString(3)
                        }));
                }
            }

            return intros;
        }



        public async Task<List<PageQuickLinksModel>> GetLibraryQuickLinksAsync(string libraryName)
        {
            //I'm going to try to cache the list thats created to reduce the database traffic.
            if (_cache.TryGetValue(libraryName, out List<PageQuickLinksModel>? cachedlinks))
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
                    var quicklinks = JsonSerializer.Deserialize<List<PageQuickLinksModel>>(json);
                    if (quicklinks != null && quicklinks.Count != 0)
                    {
                        _cache.Set(libraryName, quicklinks, TimeSpan.FromMinutes(30));
                        return quicklinks;
                    }
                }
            }
            return Error_GetLibraryLinksAsync();
        }

        public async Task<List<SubjectModel>> MakeSubLayoutNavigationLinksAsync(uint topicID)
        {
            //string[] folders = path.Split('\\');

            string category = string.Empty;// folders[folders.Length - 3];
            string library = string.Empty; // folders[folders.Length - 2];
            string subject = string.Empty;
            string topic = string.Empty;

            MySqlCommand names = new QueryHelper().SelectMapName(topicID).JoinMap().Build(_connection);
            using (var reader = names.ExecuteReader())
            {
                if (reader.Read())
                {
                    category = reader.GetString(0);
                    library = reader.GetString(1);
                    subject = reader.GetString(2);
                    topic = reader.GetString(3);
                }
            }

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
                    //string cacheKey = $"{library}_subjects";
                    //_cache.Set(cacheKey, SubjectModel_List, TimeSpan.FromMinutes(30));
                    return SubjectModel_List;
                }


            }
            return Error_GetSubjectListAsync();
        }

        public async Task<SubLayoutNavigationModel> MakeSubLayoutNavigationLinksAsync(uint topicID, uint languageID)
        {
            

            string category = string.Empty;// folders[folders.Length - 3];
            string library = string.Empty; // folders[folders.Length - 2];
            string subject = string.Empty;
            string topic = string.Empty;

            MySqlCommand names = new QueryHelper().SelectMapName(topicID).JoinMap().Build(_connection);
            using (var reader = await names.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    category = reader.GetString(0);
                    library = reader.GetString(1);
                    subject = reader.GetString(2);
                    topic = reader.GetString(3);
                }
            }


            MySqlCommand command = new QueryHelper().Select("s.subject_name, t.topic_name, sl.alias_name, tl.alias_name, t.id, t.subject_id")
                .From("topics t")
                .Join("subjects s ON s.id = t.subject_id")
                .Join("libraries l ON l.id = s.library_id")
                .Join("subjects_languages sl ON sl.map_id = s.id")
                .Join("topics_languages tl on tl.map_id = t.id")
                .Where("l.library_name", library)
                .WhereAnd("sl.language_id", languageID)
                .WhereAnd("tl.language_id", languageID)
                .Order("s.priority, t.priority")
                .Build(_connection);




            using (var reader = await command.ExecuteReaderAsync())
            {
                //List<SubjectModel> SubjectModel_List = new List<SubjectModel>();
                SubLayoutNavigationModel model = new SubLayoutNavigationModel(languageID);
                string subject_temp = string.Empty;
                int iterator = -1;
                while (await reader.ReadAsync())
                {
                    if (reader.GetUInt32(4) == topicID)
                    {
                        model.topic = new ID_String_Alias_Pair_Model
                        {
                            id = topicID,
                            text = reader.GetString(1),
                            alias = reader.GetString(3)
                        };
                        model.subject = new ID_String_Alias_Pair_Model
                        {
                            id = reader.GetFieldValue<uint>(5),
                            text = reader.GetString(0),
                            alias = reader.GetString(2)
                        };
                    }

                    string subjectName = reader.GetString(0);

                    if (subjectName != subject_temp)
                    {
                        iterator++;
                        subject_temp = subjectName;

                        model.Links.Add(
                            new SubjectModel()
                            {
                                Name = reader.GetString(2),
                                Topics =
                                {
                                    new TopicModel()
                                    {
                                        Name = reader.GetString(3),
                                        PathUrl = $"/Library/{category}/{library}/{reader.GetString(0)}/{reader.GetString(1)}"
                                    }
                                }

                            });
                    }
                    else
                    {
                        model.Links[iterator].Topics.Add(new TopicModel
                        {
                            Name = reader.GetString(3),
                            PathUrl = $"/Library/{category}/{library}/{reader.GetString(0)}/{reader.GetString(1)}"

                        });
                    }


                }
                if (model.Links != null && model.Links.Count > 0)
                {

                    return model;
                }
            }
            return Error_GetSubLayoutNavigationLinksAsync(0);

        }

        public Task UpdateMapAliasAsync(string targetTable, uint mapID, uint languageID, string aliasName, string aliasDescription)
        {
            MySqlCommand command = new QueryHelper().UpdateAlias(targetTable, mapID, languageID)
                .SetAlias(aliasName, aliasDescription).Build(_connection);

            return command.ExecuteNonQueryAsync();
        }

        public Task UpdateMapAliasNameAsync(string targetTable, uint mapID, uint languageID, string aliasName)
        {
            MySqlCommand command = new QueryHelper().UpdateAlias(targetTable, mapID, languageID)
                .SetAlias(aliasName).Build(_connection);

            return command.ExecuteNonQueryAsync();
        }

        private SubLayoutNavigationModel Error_GetSubLayoutNavigationLinksAsync(uint language)
        {
            return new SubLayoutNavigationModel(language)
            {
                topic = new ID_String_Alias_Pair_Model
                {
                    id = 0,
                    text = "ERROR",
                    alias = "Alias Error"
                },
                subject = new ID_String_Alias_Pair_Model
                {
                    id = 0,
                    text = "ERROR",
                    alias = "Alias Error"
                },
                Links = Error_GetSubjectListAsync()
            };
        }   

        private List<PageQuickLinksModel> Error_GetLibraryLinksAsync()
        {
            return new List<PageQuickLinksModel>
            {
                new PageQuickLinksModel
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


        /// <summary>
        /// Uses the Map and table suffix to get the records from a table that begins with "post_type_"
        /// Available suffixes are "video", "website", "article"
        /// </summary>
        /// <returns></returns>
        public async Task<List<ID_String_Alias_Pair_Model>> GetPostTypeRecord(PageDataMap map, string posttype)
        {
            List<ID_String_Alias_Pair_Model> links = new List<ID_String_Alias_Pair_Model>();

            MySqlCommand command = new QueryHelper().SelectPostRecord("website")
                .JoinPosts(map._topicID, map._languageID).Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                links.Add(new ID_String_Alias_Pair_Model
                {
                    id = reader.GetFieldValue<uint>(0),
                    text = reader.GetString(1),
                    alias = reader.GetString(2)
                });

            }
            return links;
        }
    }
}

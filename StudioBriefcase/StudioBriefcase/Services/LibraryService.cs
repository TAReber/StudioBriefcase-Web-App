using StudioBriefcase.Models;
using MySqlConnector;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace StudioBriefcase.Services
{
    public class LibraryService : ILibraryService
    {
        PostTypeService _postTypeService;
        private readonly MySqlConnection _connection;
        private readonly IMemoryCache _cache;
        private readonly string _tagsCacheKey;


        public LibraryService(IMemoryCache cache, MySqlConnection connection, PostTypeService postTypeService)
        {
            _connection = connection;
            _cache = cache;
            _postTypeService = postTypeService;
            _tagsCacheKey = "tags";


        }


        /// <summary>
        /// Used to Retrieve Quick LInks in Library Navigator
        /// </summary>
        /// <param name="libraryName"></param>
        /// <returns></returns>
        public async Task<List<LibraryLinksModel>> GetLibraryLinksAsync(string libraryName)
        {
            //I'm going to try to cache the list thats created to reduce the database traffic.
            if (_cache.TryGetValue(libraryName, out List<LibraryLinksModel>? cachedlinks))
            {
                if (cachedlinks != null)
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

        /// <summary>
        /// Used to Get a List of Subject and Topics that exist in a library.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Updates the Library Quick Links
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
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



        public async Task<VideoDatabaseModel> GetVideoMapData(string url)
        {

            VideoDatabaseModel map = new VideoDatabaseModel();
            await _connection.OpenAsync();


            try
            {
                var query = new MySqlCommand("SELECT t.topic_name, s.subject_name, l.library_name, c.category_name, tp.type_name, lg.lang, v.channel_name, v.channel_id, p.git_id, p.section, p.id FROM post_type_video v  JOIN posts p ON v.post_id = p.id JOIN topics t ON p.topics_id = t.id JOIN subjects s ON t.subject_id = s.id JOIN libraries l ON s.library_id = l.id JOIN categories c ON l.category_id = c.id JOIN post_types tp ON tp.id = p.post_type_id JOIN languages lg ON lg.id = p.post_language_id WHERE v.link = @link;", _connection);
                query.Parameters.AddWithValue("@link", url);
                using (var reader = await query.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {

                        map.sectionValue = reader.GetUInt32("section");
                        map.topicName = reader.GetString("topic_name");
                        map.subjectName = reader.GetString("subject_name");
                        map.libraryName = reader.GetString("library_name");
                        map.categoryName = reader.GetString("category_name");
                        map.post_type = reader.GetString("type_name");
                        map.weblink = url;
                        map.language = reader.GetString("lang");
                        map.channelName = reader.GetString("channel_name");
                        map.channelID = reader.GetString("channel_id");
                        map.gitID = reader.GetUInt32("git_id");
                        map.postID = reader.GetUInt32("id");
                        map.videoTags = new List<string>();


                    }
                }

            }
            catch
            {
                Console.WriteLine("Failed");
            }
            finally
            {

                await _connection.CloseAsync();
            }

            return map;
        }

        public async Task<List<string>> GetPostTagsAsync(uint postID)
        {
            List<string> strings = new List<string>();
            await _connection.OpenAsync();

            try
            {
                var tagquery = new MySqlCommand("SELECT tag FROM tags JOIN tags_posts ON tags.id = tags_posts.tag_id WHERE tags_posts.post_id = @postid;", _connection);
                tagquery.Parameters.AddWithValue("@postid", postID);

                using (var tagreader = await tagquery.ExecuteReaderAsync())
                {
                    while (await tagreader.ReadAsync())
                    {
                        string tag = tagreader.GetFieldValue<string>(0);
                        strings.Add(tag);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await _connection.CloseAsync();
            }

            return strings;
        }
        public async Task<LibraryTagsListModel> GetLibraryTagsAsync()
        {
            //_cache.Remove(_tagsCacheKey);

            if (_cache.TryGetValue(_tagsCacheKey, out LibraryTagsListModel? cachedtags))
            {
                if (cachedtags != null)
                {
                    return cachedtags;
                }
            }


            LibraryTagsListModel tagLists = new LibraryTagsListModel();

            await _connection.OpenAsync();
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

                    _cache.Set(_tagsCacheKey, tagLists, TimeSpan.FromMinutes(300));
                }
            }
            await _connection.CloseAsync();
            return tagLists;
        }
        public async Task AddTag(string tagname)
        {
            //TODO CHECK if Tag Already Exists.
            await _connection.OpenAsync();
            var query = new MySqlCommand($"INSERT INTO tags (tag) VALUES (@name_of_tag);", _connection);
            query.Parameters.AddWithValue("@name_of_tag", tagname);

            await query.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            _cache.Remove(_tagsCacheKey);
        }
        public async Task<bool> VideoPostTypeExistsAsync(string videoUrl)
        {
            bool exists = false;

            await _connection.OpenAsync();


            var query = new MySqlCommand($"SELECT count(1) FROM post_type_video WHERE link = @link;", _connection);
            query.Parameters.AddWithValue("@link", videoUrl);

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

        public async Task<string> DeletePost(uint postID, uint gitID)
        {
            Console.WriteLine($"{postID}, {gitID}");
            string message = string.Empty;
            await _connection.OpenAsync();


            MySqlCommand getpostTypequery;
            if (postID == 0)
            {
                getpostTypequery = new MySqlCommand("SELECT post_type_id FROM posts where id = @pid;", _connection);
            }
            else
            {
                getpostTypequery = new MySqlCommand("SELECT post_type_id FROM posts WHERE id = @pid AND(posts.git_id = 0 OR posts.git_id = @uid);", _connection);
                getpostTypequery.Parameters.AddWithValue("@uid", gitID);
            }
            getpostTypequery.Parameters.AddWithValue("@pid", postID);

            var postTypeReader = await getpostTypequery.ExecuteScalarAsync();
            if (postTypeReader != null && postTypeReader != DBNull.Value)
            {
                uint posttype = Convert.ToUInt32(postTypeReader);

                string table = string.Empty;
                switch (posttype)
                {
                    case 5:
                        table = "post_type_video";
                        break;
                    default:
                        table = string.Empty;
                        break;
                }

                if (table != string.Empty)
                {
                    using (var transaction = await _connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var tagquery = new MySqlCommand("delete From tags_posts where post_id = @pid;", _connection, transaction);
                            tagquery.Parameters.AddWithValue("@pid", postID);
                            await tagquery.ExecuteNonQueryAsync();
                            tagquery.Dispose();


                            var dataquery = new MySqlCommand($"delete from {table} where post_id = @pid", _connection, transaction);
                            dataquery.Parameters.AddWithValue("@pid", postID);
                            await dataquery.ExecuteNonQueryAsync();
                            dataquery.Dispose();


                            var postquery = new MySqlCommand("delete from posts where id = @pid;", _connection, transaction);
                            postquery.Parameters.AddWithValue("@pid", postID);
                            await postquery.ExecuteNonQueryAsync();
                            postquery.Dispose();


                        }
                        catch
                        {
                            message = "Failed to Delete Post";
                            transaction.Rollback();
                        }
                        finally
                        {
                            message = "Successfully Delete Post";
                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    message = "ERROR: Failed to locate database table on DeletePost";
                }



            }
            else
            {
                // Handle the case where the post type is null or DBNull.Value
                message = "Permission Access Denied from Deleting Post";
            }

            await _connection.CloseAsync();




            return message;
        }

        public async Task<string> InsertYoutubeLinkAsync(PostMappingDataModel postMapper)
        {
            string outputMessage = string.Empty;
            VideoDataModel publicVideoData = await Task.Run(() => _postTypeService.GetYoutubePreview(postMapper.weblink));


            bool TagsChecked = false;

            if (publicVideoData != null) //If Video Exists on Youtube
            {
                if (publicVideoData.videoTags != null) //If it has tags
                {
                    if (await VideoPostTypeExistsAsync(postMapper.weblink) == false) //Make sure video isn't already in Database
                    {
                        TagsChecked = true;
                        //LibraryTagsListModel LibraryTags = await GetLibraryTagsAsync();
                        //Video Verification Code relied on Non-Standard tagging system which didn't work well.

                        //foreach (var item in publicVideoData.videoTags)
                        //{
                        //    Console.WriteLine(item);
                        //    string loweritem = item.ToLower();
                        //    TagsChecked = LibraryTags.Tags_normal.Any(tag => tag.tagName.ToLower() == loweritem) ||
                        //        LibraryTags.Tags_IDE.Any(tag => tag.tagName.ToLower() == loweritem) ||
                        //        LibraryTags.Tags_OS.Any(tag => tag.tagName.ToLower() == loweritem);
                        //    if (TagsChecked == true)
                        //    {
                        //        break;
                        //    }

                        //}
                        if (TagsChecked == true) //Make sure atleast one tag on youtube exists in the database.
                        {
                            //Proceed to add the link to database.
                            await _connection.OpenAsync();
                            //Transaction stores the queries and undoes them if something fails.
                            using (var transaction = await _connection.BeginTransactionAsync())
                            {
                                try
                                {
                                    //Creates a new post which is the area it should exist on the website and tags to filter stuff.
                                    var query = new MySqlCommand("INSERT INTO posts (topics_id, post_type_id, post_language_id, section) VALUE ((SELECT t.id AS topic_id FROM topics t JOIN subjects s ON t.subject_id = s.id JOIN libraries l ON s.library_id = l.id JOIN categories c ON l.category_id = c.id WHERE c.category_name = @category AND l.library_name = @library AND s.subject_name = @subject AND t.topic_name = @topic), @typeId, @languageId, @section); SELECT LAST_INSERT_ID();", _connection, transaction);
                                    query.Parameters.AddWithValue("@category", postMapper.categoryName);
                                    query.Parameters.AddWithValue("@library", postMapper.libraryName);
                                    query.Parameters.AddWithValue("@subject", postMapper.subjectName);
                                    query.Parameters.AddWithValue("@topic", postMapper.topicName);
                                    query.Parameters.AddWithValue("@section", postMapper.sectionValue);
                                    query.Parameters.AddWithValue("@typeId", postMapper.posttype);
                                    query.Parameters.AddWithValue("@languageId", postMapper.language);

                                    var readerId = await query.ExecuteScalarAsync();
                                    uint postkey = Convert.ToUInt32(readerId);

                                    //Create the data that the post maps to
                                    var addlinkquery = new MySqlCommand("insert into post_type_video (link, post_id, channel_name, channel_id) values (@link, @postid, @channelname, @channelid);", _connection, transaction);
                                    addlinkquery.Parameters.AddWithValue("@link", postMapper.weblink);
                                    addlinkquery.Parameters.AddWithValue("@postId", postkey);
                                    addlinkquery.Parameters.AddWithValue("@channelname", publicVideoData.channelName);
                                    addlinkquery.Parameters.AddWithValue("@channelid", publicVideoData.channelurl);
                                    await addlinkquery.ExecuteNonQueryAsync();


                                    //Add the post to Tags_Posts table so that users can filter out less relevent posts.
                                    StringBuilder extratags = new StringBuilder();

                                    //If Tag value 1 is for any will just be excluded from existing in the database.
                                    //To help with identifying content niche, I've added Any OS and Any IDE tags.
                                    //TODO CONVERT TAGS TO LIST OF INTS.

                                    for (int i = 0; i < postMapper.tags.Count; i++)
                                    {
                                        if (postMapper.tags[i] != 0)
                                        {
                                            if (extratags.Length != 0)
                                            {
                                                extratags.Append($", (@tag{i}, @postid)");
                                            }
                                            else
                                            {
                                                extratags.Append($" (@tag{i}, @postid)");
                                            }

                                        }
                                    }


                                    var tagpostquery = new MySqlCommand($"Insert into tags_posts (tag_id, post_id) values {extratags};", _connection, transaction);

                                    tagpostquery.Parameters.AddWithValue("@postid", postkey);
                                    for (int i = 0; i < postMapper.tags.Count; i++)
                                    {
                                        if (postMapper.tags[i] != 0)
                                        {
                                            tagpostquery.Parameters.AddWithValue($"@tag{i}", postMapper.tags[i]);
                                        }
                                    }

                                    await tagpostquery.ExecuteNonQueryAsync();

                                    transaction.Commit();
                                    outputMessage = "Successfully Added Message";
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    transaction.Rollback();
                                    outputMessage = "Database Rejected Entry Transaction.";
                                }
                                finally
                                {
                                    await _connection.CloseAsync();

                                }
                            }
                        }
                        else
                        {
                            outputMessage = "Video Tags not in Database";
                        }
                    }
                    else
                    {
                        outputMessage = "Already EXISTS";
                    }
                }
                else
                {
                    outputMessage = "Video Doesn't have tags.";
                }
            }
            else
            {
                outputMessage = "Video Doesn't Exist";
            }

            return outputMessage;
        }
        public async Task<List<string>> GetVideoListAsync(NavigationMapModel map)
        {
            List<string> videoList = new List<string>();
            await _connection.OpenAsync();
            try
            {
                StringBuilder query = new StringBuilder("SELECT v.link from post_type_video v Join posts p on p.id = v.post_id join topics t on t.id = p.topics_id join subjects s on s.id = t.subject_id join libraries l on l.id = s.library_id join categories c on c.id = l.category_id");

                StringBuilder extratags = new StringBuilder();
                //Tags with value of 0 (Any) kicked out on client side.
                if (map.tags.Count > 0)
                {
                    for (int i = 0; i < map.tags.Count; i++)
                    {
                        if (extratags.Length != 0)
                            extratags.Append($", @tag{i}");
                        else
                            extratags.Append($" AND tg.id IN (@tag{i}");
                    }
                    extratags.Append(')');

                    query.Append(" LEFT join tags_posts tp on tp.post_id = p.id LEFT join tags tg on tg.id = tp.tag_id");
                }


                query.Append(" where c.category_name = @category and l.library_name = @library and s.subject_name = @subject and t.topic_name = @topic and p.section = @section and p.post_language_id = @language and post_type_id = 5");
                if (extratags.Length > 0)
                {
                    query.Append(extratags);
                }
                query.Append(';');

                var command = new MySqlCommand(query.ToString(), _connection);
                command.Parameters.AddWithValue("@category", map.categoryName);
                command.Parameters.AddWithValue("@library", map.libraryName);
                command.Parameters.AddWithValue("@subject", map.subjectName);
                command.Parameters.AddWithValue("@topic", map.topicName);
                command.Parameters.AddWithValue("@section", map.sectionValue);
                command.Parameters.AddWithValue("@language", map.language);

                for (int i = 0; i < map.tags.Count; i++)
                {
                    command.Parameters.AddWithValue($"@tag{i}", map.tags[i]);
                }



                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        string link = reader.GetString(0);
                        videoList.Add(link);
                    }
                }


            }
            catch
            {
                //TODO:: Create a Empty List
            }
            finally
            {
                await _connection.CloseAsync();
            }



            return videoList;
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


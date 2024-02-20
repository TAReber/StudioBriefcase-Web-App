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
using StudioBriefcase.Helpers;

namespace StudioBriefcase.Services
{
    public class LibraryService : BaseService, ILibraryService
    {
        PostTypeService _postTypeService;
        protected readonly string _tagsCacheKey;


        public LibraryService(IMemoryCache cache, MySqlConnection connection, PostTypeService postTypeService) : base(cache, connection)
        {
            _postTypeService = postTypeService;
            _tagsCacheKey = "tags";

            _connection.Open();
            
        }


        public async Task<PostIdentificationsModel?> GetPostIDValues(uint postID)
        {
            PostIdentificationsModel? post = null;

            MySqlCommand command = new QueryHelper().Select("post_type_id, post_language_id, git_id, section")
                .From("posts").Where("id", postID).Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    post = new PostIdentificationsModel
                    {
                        id = postID,
                        post_type_id = reader.GetUInt32(0),
                        post_language_id = reader.GetUInt32(1),
                        git_id = reader.GetUInt32(2),
                        section = reader.GetUInt32(3)
                    };
                }
            }


            return post;
        }






        //public async Task<VideoDatabaseModel> GetVideoMapData(string url)
        //{

        //    VideoDatabaseModel map = new VideoDatabaseModel();
        //    await _connection.OpenAsync();


        //    try
        //    {
        //        var query = new MySqlCommand("SELECT t.topic_name, s.subject_name, l.library_name, c.category_name, tp.type_name, lg.lang, v.channel_name, v.channel_id, p.git_id, p.section, p.id FROM post_type_video v  JOIN posts p ON v.post_id = p.id JOIN topics t ON p.topics_id = t.id JOIN subjects s ON t.subject_id = s.id JOIN libraries l ON s.library_id = l.id JOIN categories c ON l.category_id = c.id JOIN post_types tp ON tp.id = p.post_type_id JOIN languages lg ON lg.id = p.post_language_id WHERE v.link = @link;", _connection);
        //        query.Parameters.AddWithValue("@link", url);
        //        using (var reader = await query.ExecuteReaderAsync())
        //        {
        //            if (await reader.ReadAsync())
        //            {

        //                map.sectionValue = reader.GetUInt32("section");
        //                map.topicName = reader.GetString("topic_name");
        //                map.subjectName = reader.GetString("subject_name");
        //                map.libraryName = reader.GetString("library_name");
        //                map.categoryName = reader.GetString("category_name");
        //                map.post_type = reader.GetString("type_name");
        //                map.weblink = url;
        //                map.language = reader.GetString("lang");
        //                map.channelName = reader.GetString("channel_name");
        //                map.channelID = reader.GetString("channel_id");
        //                map.gitID = reader.GetUInt32("git_id");
        //                map.postID = reader.GetUInt32("id");
        //                map.videoTags = new List<string>();


        //            }
        //        }

        //    }
        //    catch
        //    {
        //        Console.WriteLine("Failed");
        //    }
        //    finally
        //    {

        //        await _connection.CloseAsync();
        //    }

        //    return map;
        //}

        public async Task<PostTagsModel> GetPostTagsAsync(uint postID)
        {
            List<uint> tags = new List<uint>() { 0, 0, 0, 0, 0 };
            //await _connection.OpenAsync();

            try
            {
                MySqlCommand command = new QueryHelper().Select("tp.tag_id").From("tags_posts tp")
                    .Join("tags ON tags.id = tp.tag_id").Where("tp.post_id", postID).Order("tags.area").Build(_connection);


                //var tagquery = new MySqlCommand("SELECT tag FROM tags JOIN tags_posts ON tags.id = tags_posts.tag_id WHERE tags_posts.post_id = @postid;", _connection);
                //tagquery.Parameters.AddWithValue("@postid", postID);

                using (var tagreader = await command.ExecuteReaderAsync())
                {
                    int index = 0;
                    while (await tagreader.ReadAsync())
                    {
                        uint tag = tagreader.GetFieldValue<uint>(0);
                        tags[index] = tag;
                        
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED THE COMMAND THINGS");
            }
            finally
            {
                //await _connection.CloseAsync();
            }

            PostTagsModel tagIDs = new PostTagsModel();
            for (int i = 0; i < tags.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        tagIDs.OS = tags[i];
                        break;
                    case 1:
                        tagIDs.IDE = tags[i];
                        break;
                    case 2:
                        tagIDs.Tag1 = tags[i];
                        break;
                    case 3:
                        tagIDs.Tag2 = tags[i];
                        break;
                    case 4:
                        tagIDs.Tag3 = tags[i];
                        break;
                }
            }

            return tagIDs;
        }

        public async Task<bool> PostTypeExistsAsync(string site, uint posttype)
        {
            bool exists = false;
            await _connection.OpenAsync();




            await _connection.CloseAsync();
            return exists;
        }

        public async Task<uint> PostTypeExistsAsync(string weblink, string table)
        {

            //await _connection.OpenAsync();
            uint postID = 0;

            MySqlCommand command = new QueryHelper().Select("p.id").From("posts p")
                .Join($"post_type_{table} pt ON pt.post_id = p.id")
                .Where("pt.link", weblink).Build(_connection);

            using (var reader = command.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    postID = reader.IsDBNull(0) ? 0 : reader.GetUInt32(0);
                }
            }
            //await _connection.CloseAsync();
            command.Dispose();

            return postID;
        }



        public async Task<string> DeletePost(uint postID, uint gitID)
        {
            //Console.WriteLine($"{postID}, {gitID}");
            string message = string.Empty;



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






            return message;
        }

        public async Task<string> InsertYoutubeLinkAsync(ClientInsertionData data)
        {
            string outputMessage = string.Empty;
            VideoDataModel publicVideoData = await Task.Run(() => _postTypeService.GetYoutubePreview(data.url));


            bool TagsChecked = false;

            if (publicVideoData != null) //If Video Exists on Youtube
            {
                if (publicVideoData.videoTags != null) //If it has tags
                {
                    if (await PostTypeExistsAsync(data.url, "video") == 0) //Make sure video isn't already in Database
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
                            //Transaction stores the queries and undoes them if something fails.
                            using (var transaction = await _connection.BeginTransactionAsync())
                            {
                                try
                                {

                                    MySqlCommand insert = new QueryHelper().InsertPost(data.topicID, data.post_type, data.language, data.gitID, data.sectionID)
                                        .LastID().Build(_connection, transaction);


                                    var readerId = await insert.ExecuteScalarAsync();
                                    uint postkey = Convert.ToUInt32(readerId);

                                    //Create the data that the post maps to
                                    var addlinkquery = new MySqlCommand("insert into post_type_video (link, post_id, channel_name, channel_id) values (@link, @postid, @channelname, @channelid);", _connection, transaction);
                                    addlinkquery.Parameters.AddWithValue("@link", data.url);
                                    addlinkquery.Parameters.AddWithValue("@postId", postkey);
                                    addlinkquery.Parameters.AddWithValue("@channelname", publicVideoData.channelName);
                                    addlinkquery.Parameters.AddWithValue("@channelid", publicVideoData.channelurl);
                                    await addlinkquery.ExecuteNonQueryAsync();


                                    //Add the post to Tags_Posts table so that users can filter out less relevent posts.
                                    StringBuilder extratags = new StringBuilder();

                                    //If Tag value 1 is for any will just be excluded from existing in the database.
                                    //To help with identifying content niche, I've added Any OS and Any IDE tags.
                                    //TODO CONVERT TAGS TO LIST OF INTS.

                                    for (int i = 0; i < data.tags.Count; i++)
                                    {
                                        if (data.tags[i] != 0)
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
                                    for (int i = 0; i < data.tags.Count; i++)
                                    {
                                        if (data.tags[i] != 0)
                                        {
                                            tagpostquery.Parameters.AddWithValue($"@tag{i}", data.tags[i]);
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


        //Refactored to use QueryHelper
        public async Task<List<string>> GetPostLinksAsync(NavigationMapModel map)
        {


            List<string> videoList = new List<string>();

            MySqlCommand command = new QueryHelper().SelectPostLinks($"post_type_{map.postType}")
                .JoinPosts(map.topicID, map.language, map.sectionValue)
                .JoinTags(map.tags).Build(_connection);

            //MySqlCommand test = new QueryHelper().SelectPostLinks($"post_type_{map.postType}").JoinPosts("Systems_Programming", "CPP", "Getting_Started", "Introduction", map.language, map.sectionValue).JoinTags(map.tags).Build(_connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    videoList.Add(reader.GetString(0));
                }
            }

            return videoList;
        }


    }


    //public async Task<bool> VideoPostTypeExistsAsync(string videoUrl)
    //{
    //    bool exists = false;

    //    await _connection.OpenAsync();

    //    var query = new MySqlCommand($"SELECT count(1) FROM post_type_video WHERE link = @link;", _connection);
    //    query.Parameters.AddWithValue("@link", videoUrl);

    //    using (var reader = query.ExecuteReader())
    //    {
    //        if (await reader.ReadAsync())
    //        {
    //            var count = reader.GetInt32(0);
    //            if (count > 0)
    //            {
    //                exists = true;
    //                //_logger.LogError("User Already Exists in Database");
    //            }
    //        }
    //    }
    //    await _connection.CloseAsync();
    //    return exists;
    //}
}


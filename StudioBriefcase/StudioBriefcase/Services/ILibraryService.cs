using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    /// <summary>
    /// Responsibility includes Getting Table Data from the Databse and Managing the ImemoryCache
    /// </summary>
    public interface ILibraryService
    {

        Task<PostTagsModel> GetPostTagsAsync(uint postID);
        Task<uint> PostTypeExistsAsync(string site, string table);

        Task<PostIdentificationsModel?> GetPostIDValues(uint postID);

        Task<string> DeletePost(uint postID, uint gitID);



        //Post Functions        
        Task<string> InsertYoutubeLinkAsync(ClientInsertionData postMapper);
        //Verification
        //Task<bool> VideoPostTypeExistsAsync(string videoUrl); //Phase Out
        

        //Task<VideoDatabaseModel> GetVideoMapData(string url);

        Task<List<string>> GetPostLinksAsync(NavigationMapModel map);

        //Task<List<string>> GetVideoListAsync(NavigationMapModel map);
        
        //Task<LibraryVideoPostModel> GetVideoPostData(string videoUrl);


        



    }
}

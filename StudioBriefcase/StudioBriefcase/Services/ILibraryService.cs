using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    /// <summary>
    /// Responsibility includes Getting Table Data from the Databse and Managing the ImemoryCache
    /// </summary>
    public interface ILibraryService
    {
        //Global Library Functions
        
        //Task<List<CategoryModel>> GetCategoryListAsync(string categoryName);
        //Task<List<LibraryModel>> GetLibraryListAsync(string libraryName);
        //Task<List<SubjectModel>> GetSubjectListAsync(string subjectName);


        Task<List<string>> GetPostTagsAsync(uint postID);

        //Post Functions        
        Task<string> InsertYoutubeLinkAsync(PostMappingDataModel postMapper);
        //Verification
        Task<bool> VideoPostTypeExistsAsync(string videoUrl); //Phase Out
        Task<bool> PostTypeExistsAsync(string site, string table);

        Task<VideoDatabaseModel> GetVideoMapData(string url);

        Task<List<string>> GetVideoListAsync(NavigationMapModel map);
        Task<string> DeletePost(uint postID, uint gitID);
        //Task<LibraryVideoPostModel> GetVideoPostData(string videoUrl);


        



    }
}

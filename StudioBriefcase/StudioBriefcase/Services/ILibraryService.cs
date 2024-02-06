using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface ILibraryService
    {
        //Navigator Functions
        Task<List<LibraryLinksModel>> GetLibraryLinksAsync(string libraryName);
        Task<List<SubjectModel>> GetSubjectListAsync(string subjectName);
        Task SetLibraryQuickLinksAsync(string libraryName, string jsonString);

        //Verification
        Task<bool> VideoPostTypeExistsAsync(string videoUrl);


        Task<VideoDatabaseModel> GetVideoMapData(string url);
        Task<List<string>> GetVideoListAsync(NavigationMapModel map);

        //Task<LibraryVideoPostModel> GetVideoPostData(string videoUrl);


        Task AddTag(string tag);
        Task<string> InsertYoutubeLinkAsync(PostMappingDataModel postMapper);

        Task<string> DeletePost(uint postID, uint gitID);

        Task<List<string>> GetPostTagsAsync(uint postID);
        Task<LibraryTagsListModel> GetLibraryTagsAsync();
        

        
    }
}

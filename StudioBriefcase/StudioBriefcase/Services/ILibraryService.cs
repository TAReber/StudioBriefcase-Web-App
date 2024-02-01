using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface ILibraryService
    {
        
        Task<List<LibraryLinksModel>> GetLibraryLinksAsync(string libraryName);
        Task<List<SubjectModel>> GetSubjectListAsync(string subjectName);
        Task SetLibraryQuickLinksAsync(string libraryName, string jsonString);

        Task<bool> VideoPostTypeExistsAsync(string videoUrl);
        Task<VideoDatabaseModel> GetVideoMapData(string url);

        //Task<LibraryVideoPostModel> GetVideoPostData(string videoUrl);
        Task<string> DeletePost(uint postID, uint gitID);

        Task AddTag(string tag);

        Task<List<string>> GetPostTagsAsync(uint postID);
        Task<LibraryTagsListModel> GetLibraryTagsAsync();
        Task<string> InsertYoutubeLinkAsyn(PostMappingDataModel postMapper);

        
    }
}

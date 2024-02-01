using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IPostTypeService
    {       
        Task<VideoDataModel> GetYoutubePreview(string videoId);      
    }
}

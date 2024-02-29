using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IBaseService
    {

        Task<LibraryMapIDsModel> BuildMapFromTopicID(uint topicID);

        Task<SelectorListModel> GetCategoryListAsync(uint language);
        Task<SelectorListModel> GetLibraryListAsync(uint categoryID, uint language);
        Task<SelectorListModel> GetSubjectListAsync(uint libraryID, uint language);
        Task<SelectorListModel> GetTopicListAsync(uint subjectID, uint language);

        


        Task<string> GetDirectyPathMap(uint TopicID);

        Task<uint> LanguagesIDexists(string language);
        Task<SelectorListModel> GetLanguages();

        Task<LibraryTagsListModel> GetLibraryTagsAsync();
    }
}

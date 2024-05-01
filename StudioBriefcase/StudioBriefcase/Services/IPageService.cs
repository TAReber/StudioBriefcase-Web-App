using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IPageService
    {
        Task<List<PageQuickLinksModel>> GetLibraryQuickLinksAsync(string libraryName);
        Task<List<SubjectModel>> MakeSubLayoutNavigationLinksAsync(uint topicID);
        Task<SubLayoutNavigationModel> MakeSubLayoutNavigationLinksAsync(uint topicID, uint languageID);

        Task UpdateMapAliasAsync(string targetTable, uint mapID, uint languageID, string aliasName, string aliasDescription);
        Task UpdateMapAliasNameAsync(string targetTable, uint mapID, uint languageID, string aliasName);
        Task<List<Map_Alias_Pair_Model>> GetCategoryAliasIntros(uint languageID);
        Task<List<Map_Alias_Pair_Model>> GetLibraryAliasIntros(uint CategoryID, uint languageID);

        Task<List<ID_String_Alias_Pair_Model>> GetPostTypeRecord(PageDataMap map, string table_suffix);
        

        //Task<PageMapModel> GetPageMapID(string category, string library, string subject, string topic);
    }
}

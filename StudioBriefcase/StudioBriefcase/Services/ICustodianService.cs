namespace StudioBriefcase.Services
{
    public interface ICustodianService
    {
        Task SetLibraryQuickLinksAsync(string libraryName, string jsonString);
        Task AddTag(string tag);
    }
}

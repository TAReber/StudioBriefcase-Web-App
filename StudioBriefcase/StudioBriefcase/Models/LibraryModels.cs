using System.ComponentModel.DataAnnotations;

namespace StudioBriefcase.Models
{
    
    public class CategoryModel
    {

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class LibraryModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class LibraryLinksModel {
        public string SiteUrl { get; set; } = string.Empty;
        public string ImgSource { get; set; } = string.Empty;
        public string ShorthandDesc { get; set; } = string.Empty;
    }

    public class SubjectModel
    {
        public string Name { get; set; } = string.Empty;
        public List<TopicModel> Topics { get; set; } = new List<TopicModel>();

    }

    public class TopicModel
    {
        public string Name { get; set; } = string.Empty;
        public string PathUrl { get; set; } = string.Empty;    
    }

    public class LibraryLinksUpdateModel
    {
        public string LibraryName { get; set; } = string.Empty;
        public string JsonString { get; set; } = string.Empty;
    }
}

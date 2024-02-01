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

    public class LibrarySectionModel
    {
        public int sectionlocation { get; set; } = 0;
        public LibrarySectionModel(int section)
        {
            sectionlocation = section;
        }
    }

    public class LibraryVideoPostModel
    {
        public uint videoid { get; set; }
        public uint postid { get; set; }
        public string link { get; set; } = string.Empty;
        public string channelname { get; set; } = string.Empty;
        public string channelurl { get; set; } = string.Empty;
    }

    public class LibraryPostIDValuesModel
    {
        public uint id { get; set; }
        public uint topics_id { get; set; }
        public uint post_type_id { get; set; }
        public uint Language_id {  get; set; }
        public uint git_id { get; set; }
        public uint section { get; set; }
    }

    
    public class LibraryTagsModel
    {
        public uint id { get; set; } = 0;
        public string tagName { get; set; } = string.Empty;

    }

    //Used to Populate ViewComponent Shared\Components\Library\_LibraryPrebuiltTags.cshtml
    public class LibraryTagsListModel
    {
        public int pageid { get; set; } = 0;
        public List<LibraryTagsModel> Tags_normal = new List<LibraryTagsModel>();
        public List<LibraryTagsModel> Tags_IDE = new List<LibraryTagsModel>();
        public List<LibraryTagsModel> Tags_OS = new List<LibraryTagsModel>();

        
    }

}

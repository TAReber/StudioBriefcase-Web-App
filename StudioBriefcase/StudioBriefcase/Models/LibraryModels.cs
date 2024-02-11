using System.ComponentModel.DataAnnotations;

namespace StudioBriefcase.Models
{

    public class SelectorModel
    {
        public uint id { get; set; }
        public string name { get; set; } = string.Empty;
    }

    public class SelectorListModel
    {
        List<SelectorModel> list = new List<SelectorModel>();
    }

    public class LibrarySelectorIDsModel
    {
        public uint CategoryID { get; set; }
        public uint LibraryID { get; set; }
        public uint SubjectID { get; set; }
        public uint TopicID { get; set; }
    }

    public class LibraryMapListModel : LibrarySelectorIDsModel {        
        public SelectorModel Categories { get; set; } = new SelectorModel();
        public SelectorModel Libraries { get; set; } = new SelectorModel();
        public SelectorModel Subjects { get; set; } = new SelectorModel();
        public SelectorModel Topics { get; set; } = new SelectorModel();
        
        public LibraryMapListModel(LibrarySelectorIDsModel ids)
        {
            CategoryID = ids.CategoryID;
            LibraryID = ids.LibraryID;
            SubjectID = ids.SubjectID;
            TopicID = ids.TopicID;
        }
    }

    /// <summary>
    /// Model used in the Index.cs pages, To be phased out for datadriven approach
    /// </summary>
    public class staticlibraryLinkModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    //public class CategoryModel
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public string Description { get; set; } = string.Empty;
    //}

    //public class LibraryModel
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public string Description { get; set; } = string.Empty;
    //}

    //Model used to update the quicklinks in the database with json string
    public class LibraryQuickLinksUpdaterModel
    {
        public string LibraryName { get; set; } = string.Empty;
        public string JsonString { get; set; } = string.Empty;
    }

    public class LibraryQuickLinksModel {
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

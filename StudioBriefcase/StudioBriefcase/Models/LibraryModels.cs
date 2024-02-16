using System.ComponentModel.DataAnnotations;

namespace StudioBriefcase.Models
{

    /// <summary>
    /// Model used in the Index.cs pages, To be phased out for datadriven approach
    /// </summary>
    public class staticlibraryLinkModel //Good
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ID_String_Pair_Model
    {
        public uint id { get; set; }
        public string text { get; set; } = string.Empty;
    }

    public class SelectorListModel
    {
        public List<ID_String_Pair_Model> list = new List<ID_String_Pair_Model>();
    }

    public class LibraryMapIDsModel
    {
        public uint CategoryID { get; set; }
        public uint LibraryID { get; set; }
        public uint SubjectID { get; set; }
        public uint TopicID { get; set; }
    }

    public class LibraryMapListModel : LibraryMapIDsModel {        
        public SelectorListModel Categories { get; set; } = new SelectorListModel();
        public SelectorListModel Libraries { get; set; } = new SelectorListModel();
        public SelectorListModel Subjects { get; set; } = new SelectorListModel();
        public SelectorListModel Topics { get; set; } = new SelectorListModel();
        
        public LibraryMapListModel(LibraryMapIDsModel ids)
        {
            CategoryID = ids.CategoryID;
            LibraryID = ids.LibraryID;
            SubjectID = ids.SubjectID;
            TopicID = ids.TopicID;
        }
    }

    /// <summary>
    /// Primarily Used to for Displaying Tags on the User Interface
    /// </summary>
    public class PostTagsModel
    {
        public uint OS = 0;
        public uint IDE = 0;
        public uint Tag1 = 0;
        public uint Tag2 = 0;
        public uint Tag3 = 0;
    }

    //Used to Populate ViewComponent Shared\Components\Library\_LibraryPrebuiltTags.cshtml
    public class LibraryTagsListModel
    {       
        //public LibraryTagsIDsModel ids { get; set; } = new LibraryTagsIDsModel();
        public SelectorListModel Tags_normal { get; set; } = new SelectorListModel();
        public SelectorListModel Tags_IDE { get; set; } = new SelectorListModel();
        public SelectorListModel Tags_OS { get; set; } = new SelectorListModel();
    }

    public class TagsModel
    {
        public PostTagsModel ids { get; set; } = new PostTagsModel();
        public LibraryTagsListModel TagLists { get; set; } = new LibraryTagsListModel();
    }

    public class PostInspectModel
    {
        public bool exists = false;
        public string WebSite_URL { get; set; } = string.Empty;
        
        public LibraryMapListModel? Map { get; set; } = null;
        public TagsModel Tags { get; set; } = new TagsModel();
        public PostIdentificationsModel Post { get; set; } = new PostIdentificationsModel();

        public PostInspectModel(string url)
        {
            WebSite_URL = url;
        }

    }

    public class PostIdentificationsModel
    {
        public uint id { get; set; } = 0;
        public uint post_type_id { get; set; } = 1;
        public uint post_language_id { get; set; } = 1;
        public uint git_id { get; set; } = 0;
        public uint section { get; set; } = 0;
    }


    public class ClientSideSendingData
    {
        public uint topicID { get; set; } = 0;
        public uint sectionID { get; set; } = 0;
        public string url { get; set; } = string.Empty;
        public uint language { get; set; } = 1;
        public uint post_type { get; set; } = 5;

    }

    public class ClientInsertionData : ClientSideSendingData
    {
        public uint gitID { get; set; } = 0;
        public List<uint> tags { get; set; } = new List<uint>();
    }

    //public class LibraryTagsModel
    //{
    //    LibraryTagsIDsModel Tags = new LibraryTagsIDsModel();
    //    LibraryTagsListModel Lists = new LibraryTagsListModel();
    //}

    //public class LibraryTagsModel
    //{
    //    public uint id { get; set; } = 0;
    //    public string tagName { get; set; } = string.Empty;

    //}

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

    


}

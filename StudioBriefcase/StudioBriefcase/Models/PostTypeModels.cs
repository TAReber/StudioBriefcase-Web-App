using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace StudioBriefcase.Models
{
    //public class OneParamStringModel
    //{
    //    public string text = string.Empty;
    //}

    //public class VideoPreviewFetchModel
    //{
    //    public string videolink { get; set; } = string.Empty;
    //    public uint section { get; set; } = 0;
    //}


    ///<summary>
    /// Post Data needed to Retrieve a list of Posts
    /// </summary>
    public class NavigationMapModel
    {
        public uint sectionValue { get; set; }
        public uint topicID { get; set; }
        public string postType { get; set; } = string.Empty;

        public uint language { get; set; }
        public List<uint> tags { get; set; } = new List<uint>();
    }

    //public class PostMappingDataModel : BaseMapModel
    //{

    //    public int language { get; set; } = 1;
    //    //public string weblink {  get; set; } = string.Empty;
    //    public uint posttype { get; set; }
    //    public uint GitID { get; set; } = 0;
    //    public List<uint> tags { get; set; } = new List<uint>();

        
    //}

    /// <summary>
    /// Not sure what this is used for at the moment
    /// </summary>
    public class BaseMapModel
    {
        public string weblink { get; set; } = string.Empty;
        public uint sectionValue { get; set; }
        public string topicName { get; set; } = string.Empty;
        public string subjectName { get; set; } = string.Empty;
        public string libraryName { get; set; } = string.Empty;
        public string categoryName { get; set; } = string.Empty;
    }

    public class IdentifiersModel : BaseMapModel
    {
        
        public uint postID { get; set; }
        public uint gitID { get; set; }
        public string post_type { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public List<string>? videoTags = null;
    }

    public class VideoDatabaseModel : IdentifiersModel
    {
        public string channelName { get; set; } = string.Empty;
        public string channelID { get; set; } = string.Empty;
    }


    /// <summary>
    /// The Model Used to Store Scraped data from the Youtube API
    /// </summary>
    public class VideoDataModel
    {
        public uint sectionID { get; set; } = 0;
        public string title { get; set; } = string.Empty;
        public string videourl { get; set; } = string.Empty;
        public string channelName { get; set; } = string.Empty;
        public string channelurl { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<string>? videoTags = null;

        public VideoDataModel() { }

        public VideoDataModel(VideoDataModel model)
        {
            title = model.title;
            videourl = model.videourl;
            channelName = model.channelName;
            channelurl = model.channelurl;
            thumbnail = model.thumbnail;
            description = model.description;
            videoTags = model.videoTags != null ? new List<string>(model.videoTags) : new List<string>();
        }
    }

    //public class TagNameModel
    //{
    //    public string tagName { get; set; } = string.Empty;
    //}
}

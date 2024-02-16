using StudioBriefcase.Models;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace StudioBriefcase.Services
{
    public class PostTypeService : IPostTypeService
    {
        IHttpClientFactory _clientFactory;
        IConfiguration _configuration;

        public PostTypeService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<VideoDataModel> GetYoutubePreview(string videoId)
        {

            VideoDataModel? videoData = null;
            var apiKey = _configuration["youtube-api-key"];

            var vidID = videoId.Split("/").Last();
            var apiCode = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={vidID}&key={apiKey}";

            using (var httpClient = _clientFactory.CreateClient("youtube"))
            {
                
                try
                {
                    var response = httpClient.GetAsync(apiCode).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        using JsonDocument doc = JsonDocument.Parse(result);
                        JsonElement element = doc.RootElement;
                        //Console.WriteLine(result);

                        videoData = new VideoDataModel
                        {
                            title = element.GetProperty("items")[0].GetProperty("snippet").GetProperty("title").ToString(),
                            channelName = element.GetProperty("items")[0].GetProperty("snippet").GetProperty("channelTitle").ToString(),
                            thumbnail = element.GetProperty("items")[0].GetProperty("snippet").GetProperty("thumbnails").GetProperty("default").GetProperty("url").ToString(),
                            description = element.GetProperty("items")[0].GetProperty("snippet").GetProperty("description").ToString(),
                            channelurl = $"https://www.youtube.com/channel/{element.GetProperty("items")[0].GetProperty("snippet").GetProperty("channelId")}",
                            videourl = videoId,
                            videoTags = element.GetProperty("items")[0].GetProperty("snippet").GetProperty("tags").Deserialize<List<string>>()
                        };


                    }

                }
                catch
                {
                    Console.WriteLine($"Failed to Find Video through the YOUTUBE API, {videoId}");
                }

            }

            return videoData != null ? await Task.FromResult(new VideoDataModel(videoData)) : null;
        }


    }
}

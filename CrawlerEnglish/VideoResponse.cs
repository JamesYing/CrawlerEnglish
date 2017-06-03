using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlerEnglish
{
    public class VideoResponse
    {
        public string Status { get; set; }

        public string Content { get; set; }
    }

    public class Video
    {
        [JsonProperty("otitle")]
        public string Title { get; set; }

        [JsonProperty("videourl")]
        public string Url { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("ctime")]
        public string Time { get; set; }
    }
}

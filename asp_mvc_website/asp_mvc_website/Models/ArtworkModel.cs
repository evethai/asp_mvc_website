﻿using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace asp_mvc_website.Models
{
    public class ArtworkModel
    {
        public int artworkId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; }
        public double Price { get; set; }
        public DateTime? CreateOn { get; set; }
        public string Image { get; set; } = string.Empty;

        [JsonProperty("categories")]
        public List<int> categories { get; set; } = new List<int>();


        [JsonProperty("ImageUrl")]
        private List<string> ImageUrl { get; set; } = new List<string>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ImageUrl != null && ImageUrl.Count > 0)
            {
                Image = ImageUrl[0];
            }
        }
    }
}

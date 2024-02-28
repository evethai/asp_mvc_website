using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace asp_mvc_website.Models
{
    public class ArtworkModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double? Price { get; set; }
        public string Image { get; set; } = string.Empty;

        [JsonProperty("ImageUrl")]
        private List<string> ImageUrl { get; set; } = new List<string>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Take the first URL from the list and assign it to ImageUrl
            if (ImageUrl != null && ImageUrl.Count > 0)
            {
                Image = ImageUrl[0];
            }
        }
    }
}

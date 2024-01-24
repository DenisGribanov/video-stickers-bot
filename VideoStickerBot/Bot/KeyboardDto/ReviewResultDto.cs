using Newtonsoft.Json;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.KeyboardDto
{
    public class ReviewResultDto
    {
        [JsonProperty("r")]
        public VideoReviewEnum ReviewResult { get; set; }

        [JsonProperty("id")]
        public long VideoStickerId { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static ReviewResultDto FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewResultDto>(json);
        }

        public static ReviewResultDto Init(VideoReviewEnum reviewEnum, long VideoStickerId)
        {
            return new ReviewResultDto
            {
                ReviewResult = reviewEnum,
                VideoStickerId = VideoStickerId
            };
        }
    }
}

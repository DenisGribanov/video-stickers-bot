using Newtonsoft.Json;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.KeyboardDto
{
    public class StickerManagmentDto
    {
        [JsonProperty("a")]
        public StickerManagmentEnum Action { get; set; }

        [JsonProperty("id")]
        public long VideoStickerId { get; set; }

        internal static StickerManagmentDto Init(StickerManagmentEnum action, long id)
        {
            return new StickerManagmentDto
            {
                Action = action,
                VideoStickerId = id
            };
        }
    }
}

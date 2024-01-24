using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.KeyboardDto
{
    public class StikerSortDto
    {
        public SortEnum Sort { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static StikerSortDto FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<StikerSortDto>(json);
        }

        public static StikerSortDto Init(SortEnum sortEnum)
        {
            return new StikerSortDto
            {
                Sort = sortEnum
            };
        }
    }
}

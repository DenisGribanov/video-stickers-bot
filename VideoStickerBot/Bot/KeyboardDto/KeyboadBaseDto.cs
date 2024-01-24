using Newtonsoft.Json;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.KeyboardDto
{
    public class KeyboadBaseDto<T>
    {
        [JsonProperty("t")]
        public KeyboardTypeEnum Type { get; set; }

        [JsonProperty("d")]
        public T Data { get; set; }

        public static KeyboadBaseDto<T> Init(KeyboardTypeEnum typeEnum, T data)
        {
            return new KeyboadBaseDto<T>
            {
                Data = data,
                Type = typeEnum
            };
        }

        public static string InitJson(KeyboardTypeEnum typeEnum, T data)
        {
            var dto = new KeyboadBaseDto<T>
            {
                Data = data,
                Type = typeEnum
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        }

        public static KeyboadBaseDto<T> FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<KeyboadBaseDto<T>>(json);
        }

    }
}

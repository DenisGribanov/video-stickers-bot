namespace VideoStickerBot.Services.TelegramIntegration
{
    public class TelegramInlineQueryResultVideo
    {
        public string UniqueId { get; set; } 
        public string VideoUrl { get; set; }

        public string ThumbUrl { get; set; }

        public string Title { get; set; }

        public string Caption { get; set; }

        public TelegramInlineQueryResultVideo(string uniqueId, string videoUrl, string thumbUrl, string title, string caption)
        {
            UniqueId = uniqueId;
            VideoUrl = videoUrl;
            ThumbUrl = thumbUrl;
            Title = string.IsNullOrEmpty(title) ? "id:" + uniqueId : title;
            Caption = caption;
        }
    }
}

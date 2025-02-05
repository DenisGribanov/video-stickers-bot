namespace VideoStickerBot.Database
{
    public partial class VideoStickersStat
    {
        public long Id { get; set; }
        public long UserChatId { get; set; }
        public long StickerId { get; set; }
        public int ClickCount { get; set; }

        public virtual VideoSticker Sticker { get; set; } = null!;
        public virtual TgUser UserChat { get; set; } = null!;
    }
}
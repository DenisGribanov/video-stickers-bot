namespace VideoStickerBot.Database
{
    public partial class ChannelPost
    {
        public long Id { get; set; }
        public long VideoStickerId { get; set; }
        public int MessageId { get; set; }
        public string VideoUrl { get; set; } = null!;
        public long ChannelId { get; set; }
        public int? ReplyMessageId { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateAdd { get; set; }
        public string? PostText { get; set; }

        public virtual Channel Channel { get; set; } = null!;
        public virtual VideoSticker VideoSticker { get; set; } = null!;
    }
}
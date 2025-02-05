namespace VideoStickerBot.Database
{
    public partial class CheckingVideoSticker
    {
        public long Id { get; set; }
        public long VideoStickerId { get; set; }
        public long ModeratorChatId { get; set; }
        public DateTime DateAdd { get; set; }
        public int? Status { get; set; }
        public DateTime? StatusUpdateTime { get; set; }
        public string? Description { get; set; }
        public bool Deleted { get; set; }

        public virtual TgUser ModeratorChat { get; set; } = null!;
        public virtual VideoSticker VideoSticker { get; set; } = null!;
    }
}
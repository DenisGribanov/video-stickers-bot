namespace VideoStickerBot.Database
{
    public partial class Channel
    {
        public Channel()
        {
            ChannelPosts = new HashSet<ChannelPost>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public bool CanPostMessages { get; set; }
        public bool CanEditMessages { get; set; }
        public bool CanDeleteMessages { get; set; }
        public string? Status { get; set; }
        public int ChannelType { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<ChannelPost> ChannelPosts { get; set; }
    }
}
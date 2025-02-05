using System.Text;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Database
{
    /// <summary>
    /// видео стикеры для бота
    /// </summary>
    public partial class VideoSticker
    {
        public VideoSticker()
        {
            ChannelPosts = new HashSet<ChannelPost>();
            CheckingVideoStickers = new HashSet<CheckingVideoSticker>();
            VideoStickersStats = new HashSet<VideoStickersStat>();
        }

        public long Id { get; set; }
        public int VideoDuration { get; set; }
        public string FileId { get; set; } = null!;
        public string? FileUniqueId { get; set; }
        public string? Description { get; set; }
        public string? Hashtags { get; set; }
        public long AuthorChatId { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedDescription { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int MessageId { get; set; }
        public DateTime DateAdd { get; set; }

        public virtual TgUser AuthorChat { get; set; } = null!;
        public virtual ICollection<ChannelPost> ChannelPosts { get; set; }
        public virtual ICollection<CheckingVideoSticker> CheckingVideoStickers { get; set; }
        public virtual ICollection<VideoStickersStat> VideoStickersStats { get; set; }

        public bool IsPublished()
        {
            return ChannelPosts.Any(x => !x.Deleted && x.Channel.ChannelType == (int)ChannelType.PUBLIC);
        }

        public ChannelPost? GetPrivateChannelPost()
        {
            return ChannelPosts.First(x => !x.Deleted && x.Channel.ChannelType == (int)ChannelType.PRIVATE_REVIEW);
        }

        public ChannelPost? GetPublicChannelPost()
        {
            return ChannelPosts.First(x => !x.Deleted && x.Channel.ChannelType == (int)ChannelType.PUBLIC);
        }

        public List<string> GetHashTags()
        {
            var spl = Hashtags?.Split(' ');
            return spl == null ? new List<string>() : spl.Select(x => "#" + x.Trim().ToLower()).ToList();
        }

        public string BuildPostDescriptionText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var tag in GetHashTags())
            {
                sb.Append($"{tag} ");
            }

            string hashTagsLine = sb.ToString();

            string descriptionText = Description + "\n" + hashTagsLine + "\n\n_Для быстрого поиска:_ " + Id;

            return descriptionText;
        }
        public Dictionary<string, string> GetReviewKeybooardData()
        {
            var approval = KeyboadBaseDto<ReviewResultDto>.InitJson(KeyboardTypeEnum.VIDEO_REVIEW_RESULT,
                ReviewResultDto.Init(VideoReviewEnum.APPROVAL, Id));

            var rejection = KeyboadBaseDto<ReviewResultDto>.InitJson(KeyboardTypeEnum.VIDEO_REVIEW_RESULT,
                 ReviewResultDto.Init(VideoReviewEnum.REJECTION, Id));

            return new Dictionary<string, string>
            {
                { "1 - ✅", approval },

                { "2 - ❌", rejection },
            };
        }

        public int TotalClick()
        {
            if (VideoStickersStats == null || VideoStickersStats.Count == 0)
                return 0;

            return VideoStickersStats.Sum(s => s.ClickCount);
        }
    }
}
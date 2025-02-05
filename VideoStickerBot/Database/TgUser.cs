using VideoStickerBot.Enums;

namespace VideoStickerBot.Database
{
    public partial class TgUser
    {
        public TgUser()
        {
            CheckingVideoStickers = new HashSet<CheckingVideoSticker>();
            VideoStickers = new HashSet<VideoSticker>();
            VideoStickersStats = new HashSet<VideoStickersStat>();
        }

        public long ChatId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? LanguageCode { get; set; }
        public DateTime CreateData { get; set; }
        public int SortedType { get; set; }
        public int UserRole { get; set; }
        public bool UploadDisabled { get; set; }

        public virtual ICollection<CheckingVideoSticker> CheckingVideoStickers { get; set; }
        public virtual ICollection<VideoSticker> VideoStickers { get; set; }
        public virtual ICollection<VideoStickersStat> VideoStickersStats { get; set; }

        public bool UserIsAdmin()
        {
            return (int)UserRoleEnum.ADMIN == UserRole;
        }

        public bool UserIsReviewer()
        {
            return (int)UserRoleEnum.REVIEWER == UserRole;
        }
    }
}
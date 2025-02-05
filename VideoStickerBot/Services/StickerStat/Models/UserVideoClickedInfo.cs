using VideoStickerBot.Database;

namespace VideoStickerBot.Services.StickerStat.Models
{
    public class UserVideoClickedInfo
    {
        public VideoSticker Video;
        public int UserClickCount;

        public UserVideoClickedInfo()
        {
        }

        public UserVideoClickedInfo(VideoSticker video, int userClickCount)
        {
            Video = video;
            UserClickCount = userClickCount;
        }
    }
}
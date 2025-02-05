using VideoStickerBot.Database;

namespace VideoStickerBot.Services.StickerPublishing
{
    public interface IStickerPublishing
    {
        Task Publish(VideoSticker _sticker);
    }
}
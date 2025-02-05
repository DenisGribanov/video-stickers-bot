using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search
{
    public interface ISearchSticker
    {
        IEnumerable<VideoSticker> Search(string query);
    }
}
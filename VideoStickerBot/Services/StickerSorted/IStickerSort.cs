using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Services.StickerSorted
{
    public interface IStickerSort
    {
        SortEnum SortType { get; }

        List<VideoSticker> Sort();
    }
}

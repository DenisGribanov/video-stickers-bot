using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;

namespace VideoStickerBot.Services.StickerSorted
{
    public class NewestSorted : IStickerSort
    {
        public SortEnum SortType => SortEnum.NEWEST;

        private readonly IDataStore dataStore;

        public NewestSorted(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public List<VideoSticker> Sort()
        {
            return dataStore.GetVideoStickers().OrderByDescending(x => x.Id).ToList();
        }
    }
}
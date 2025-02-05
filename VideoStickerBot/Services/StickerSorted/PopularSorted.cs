using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerStat;

namespace VideoStickerBot.Services.StickerSorted
{
    public class PopularSorted : IStickerSort
    {
        public SortEnum SortType => SortEnum.POPULAR;

        private TotalStat TotalStat { get; set; }

        private readonly IDataStore dataStore;

        public PopularSorted(IDataStore dataStore)
        {
            this.dataStore = dataStore;
            this.TotalStat = new TotalStat(dataStore);
        }

        public List<VideoSticker> Sort()
        {
            var stickers = dataStore.GetVideoStickers();

            var stats = TotalStat.Get().OrderByDescending(x => x.TotalClick)
                                    .Where(x => x.VideoSticker != null)
                                    .Select(x => x.VideoSticker).ToList();

            HashSet<long> ids = stats.Select(x => x.Id).ToHashSet();

            foreach (var sticker in stickers)
            {
                if (ids.Contains(sticker.Id)) continue;

                stats.Add(sticker);
                ids.Add(sticker.Id);
            }

            return stats;
        }
    }
}
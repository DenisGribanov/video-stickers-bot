using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerStat;

namespace VideoStickerBot.Services.StickerSorted
{
    public class PersonSorted : IStickerSort
    {
        public SortEnum SortType => SortEnum.PERSON_RANKING;

        private readonly PersonsStat PersonsStat;

        private readonly IDataStore dataStore;

        public PersonSorted(IDataStore dataStore, long UserChatId)
        {
            this.dataStore = dataStore;
            PersonsStat = new PersonsStat(dataStore, UserChatId);
        }

        public List<VideoSticker> Sort()
        {
            PersonsStat.Load();

            var stickers = dataStore.GetVideoStickers();

            var stats = PersonsStat.GetByUser()
                                    .OrderByDescending(x => x.UserClickCount)
                                    .Where(x => x.Video != null).Select(x => x.Video).ToList();

            HashSet<long> ids = stats.Select(x => x.Id).ToHashSet();

            foreach (var sticker in stickers.OrderByDescending(x => x.TotalClick()))
            {
                if (ids.Contains(sticker.Id)) continue;

                stats.Add(sticker);
                ids.Add(sticker.Id);
            }

            return stats;
        }
    }
}
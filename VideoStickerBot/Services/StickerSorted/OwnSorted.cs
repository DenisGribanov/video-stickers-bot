using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;

namespace VideoStickerBot.Services.StickerSorted
{
    public class OwnSorted : IStickerSort
    {
        public SortEnum SortType => SortEnum.OWN;

        private readonly IDataStore dataStore;

        private readonly long authorChatId;

        public OwnSorted(IDataStore dataStore, long authorChatId)
        {
            this.dataStore = dataStore;
            this.authorChatId = authorChatId;
        }

        public List<VideoSticker> Sort()
        {
            var stickers = dataStore.GetVideoStickers();

            var byAuthor = stickers.Where(x => x.AuthorChatId == authorChatId).OrderByDescending(x => x.Id).ToList();

            if (byAuthor.Count == 0) return stickers;

            var hashSet = byAuthor.Select(x => x.Id).ToHashSet();

            foreach (var sticker in stickers)
            {
                if (hashSet.Contains(sticker.Id)) continue;

                byAuthor.Add(sticker);
                hashSet.Add(sticker.Id);
            }

            return byAuthor;
        }
    }
}
using VideoStickerBot.Database;
using VideoStickerBot.Services.Search.SearchStrategy;
using VideoStickerBot.Services.StickerSorted;

namespace VideoStickerBot.Services.Search
{
    public class SearchSticker : ISearchSticker
    {
        private readonly IStickerSort stickerSort;

        public SearchSticker(IStickerSort stickerSort)
        {
            this.stickerSort = stickerSort;
        }

        public IEnumerable<VideoSticker> Search(string? query)
        {
            var strategy = GetSearchStrategies(query);

            if (strategy == null) return stickerSort.Sort();

            return strategy.Search(stickerSort.Sort());
        }

        private ISearchStrategy GetSearchStrategies(string query)
        {
            List<ISearchStrategy> searchStrategies = new List<ISearchStrategy>
            {
                new SearchStrategyFromAllFields(query),
                new SearchStrategyFromHashTag(query),
                new SearchStrategyFromId(query),
                new SearchStrategyFromEmptyQuery(query),
                new SearchStrategyFromCashTag(query)
            };

            return searchStrategies.Where(x => x.IsMatch()).FirstOrDefault();
        }
    }
}
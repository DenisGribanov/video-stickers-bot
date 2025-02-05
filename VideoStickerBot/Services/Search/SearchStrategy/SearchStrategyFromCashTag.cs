using VideoStickerBot.Constants;
using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public class SearchStrategyFromCashTag : SearchStrategyBase, ISearchStrategy
    {
        public SearchStrategyFromCashTag(string query) : base(query)
        {
        }

        public SearchStrategyEnum StrategyEnum => SearchStrategyEnum.FROM_CASHTAG;

        string ISearchStrategy.Query => base.Query;

        public bool IsMatch()
        {
            if (isMatch.HasValue) return isMatch.Value;

            isMatch = !string.IsNullOrEmpty(Query) && Query.StartsWith("$");

            return isMatch.Value;
        }

        public IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers)
        {
            if (CashTagValues.FRESH.Equals(Query))
            {
                return SourceStickers.OrderByDescending(x => x.Id);
            }
            else if (CashTagValues.BEST.Equals(Query))
            {
                return SourceStickers.OrderByDescending(x => x.TotalClick());
            }

            return SourceStickers;
        }
    }
}
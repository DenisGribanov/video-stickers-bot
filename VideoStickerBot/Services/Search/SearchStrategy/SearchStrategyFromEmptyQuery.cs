using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public class SearchStrategyFromEmptyQuery : SearchStrategyBase, ISearchStrategy
    {
        public SearchStrategyFromEmptyQuery(string query) : base(query)
        {
        }

        public SearchStrategyEnum StrategyEnum => SearchStrategyEnum.FROM_EMPTY_QUERY;

        string ISearchStrategy.Query => base.Query;

        public bool IsMatch()
        {
            if (isMatch.HasValue) return isMatch.Value;

            isMatch = string.IsNullOrEmpty(Query);

            return isMatch.Value;
        }

        public IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers)
        {
            if (!IsMatch()) return GetEmptyResult();

            return SourceStickers;
        }
    }
}
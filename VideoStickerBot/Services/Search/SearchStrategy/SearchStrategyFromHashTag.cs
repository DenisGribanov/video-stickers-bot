using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public class SearchStrategyFromHashTag : SearchStrategyBase, ISearchStrategy
    {
        public SearchStrategyFromHashTag(string query) : base(query)
        {
        }

        public SearchStrategyEnum StrategyEnum => SearchStrategyEnum.FROM_HASHTAG;

        string ISearchStrategy.Query => base.Query;

        public bool IsMatch()
        {
            if (isMatch.HasValue) return isMatch.Value;

            isMatch = !string.IsNullOrEmpty(Query) && Query.Trim().StartsWith("#");

            return isMatch.Value;
        }

        public IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers)
        {
            if (!IsMatch()) return GetEmptyResult();

            var res = SourceStickers.Where(x => x.GetHashTags().Contains(Query.ToLower()));

            if (res.Count() > 0)
            {
                return res;
            }
            else
            {
                return SourceStickers.Where(x => x.GetHashTags().Any(x => x.StartsWith(Query.ToLower())));
            }
        }
    }
}
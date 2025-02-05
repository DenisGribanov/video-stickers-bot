using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public abstract class SearchStrategyBase
    {
        protected bool? isMatch;
        protected readonly long? videoStickerId;
        protected string Query { get; private set; }

        protected SearchStrategyBase(string query)
        {
            if (query != null)
            {
                Query = query.Trim().ToLower();
            }

            videoStickerId = ParseId(Query);
        }

        protected List<VideoSticker> GetEmptyResult()
        {
            return new List<VideoSticker>();
        }

        private long? ParseId(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            long id;
            if (long.TryParse(text.Trim(), out id))
            {
                return id;
            }
            else
            {
                return null;
            }
        }
    }
}
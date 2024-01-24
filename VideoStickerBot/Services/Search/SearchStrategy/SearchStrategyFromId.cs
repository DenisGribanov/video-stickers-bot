using System.Text.RegularExpressions;
using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public class SearchStrategyFromId : SearchStrategyBase, ISearchStrategy
    {
        public SearchStrategyFromId(string query) : base(query)
        {
        }

        public SearchStrategyEnum StrategyEnum => SearchStrategyEnum.FROM_ID;

        string ISearchStrategy.Query => throw new NotImplementedException();

        

        public bool IsMatch()
        {
            if(isMatch.HasValue) return isMatch.Value; 

            isMatch = videoStickerId.HasValue && videoStickerId > 0;
            return isMatch.Value;
        }

        public IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers)
        {
            if (!IsMatch()) return new List<VideoSticker>();

            return SourceStickers.Where(x => x.Id.Equals(videoStickerId));
        }

    }
}

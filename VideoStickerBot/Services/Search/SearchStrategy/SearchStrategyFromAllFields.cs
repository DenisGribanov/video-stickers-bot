using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public class SearchStrategyFromAllFields : SearchStrategyBase, ISearchStrategy
    {
        public SearchStrategyFromAllFields(string query) : base(query)
        {
        }

        public SearchStrategyEnum StrategyEnum => SearchStrategyEnum.FROM_ALL_FIELDS;


        string ISearchStrategy.Query => base.Query;

        public bool IsMatch()
        {   
            if(isMatch.HasValue) return isMatch.Value;

            isMatch = !string.IsNullOrEmpty(Query) 
                            && !Query.Contains('#')
                            && !Query.Contains('$')
                            && !videoStickerId.HasValue;

            return isMatch.Value;
        }

        public IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers)
        {   
            if(!IsMatch()) return GetEmptyResult();

            var result = SourceStickers.Where(x =>
                        (x.Description != null && x.Description.ToLower().Contains(Query.Trim().ToLower()))

                        || x.GetHashTags().Contains(Query.ToLower())

                        || x.GetHashTags().Any(x => x.Contains(Query.ToLower()))

                        );

            return result;
        }
    }
}

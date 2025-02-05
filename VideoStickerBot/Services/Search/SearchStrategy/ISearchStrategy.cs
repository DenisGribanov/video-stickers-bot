using VideoStickerBot.Database;

namespace VideoStickerBot.Services.Search.SearchStrategy
{
    public interface ISearchStrategy
    {
        SearchStrategyEnum StrategyEnum { get; }

        string Query { get; }

        bool IsMatch();

        IEnumerable<VideoSticker> Search(IEnumerable<VideoSticker> SourceStickers);
    }
}
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Services.Search
{
    public interface ISearchSticker
    {
        IEnumerable<VideoSticker> Search(string query);

    }
}

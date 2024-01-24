using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.Interfaces
{
    public interface IStateData
    {
        TgUser? CurrentUser { get; }
        BotState? StateCurrentUser { get; }

        void LoadData();

        BotState? UpdateState(BotState value);
    }
}

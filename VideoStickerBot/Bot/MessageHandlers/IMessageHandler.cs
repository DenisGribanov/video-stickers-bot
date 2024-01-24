using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers
{
    public interface IMessageHandler
    {
        Task Handle();

        bool Match();

        BotState? UpdateStateForCurrentUser();
    }
}

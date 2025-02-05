using VideoStickerBot.Bot.MessageHandlers;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public interface IBuilderHandlerList
    {
        List<IMessageHandler> GetHandlers();

        bool MatchHandlerExist();

        IMessageHandler GetMatchHandler();
    }
}
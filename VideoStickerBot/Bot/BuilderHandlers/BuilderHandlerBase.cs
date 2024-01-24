using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public abstract class BuilderHandlerBase : IBuilderHandlerList
    {
        protected readonly IBotSubSystems botSubSystems;

        protected readonly List<IMessageHandler> handlers = new List<IMessageHandler>();

        public BuilderHandlerBase(IBotSubSystems botSubSystems)
        {
            this.botSubSystems = botSubSystems;
        }


        public List<IMessageHandler> GetHandlers()
        {
            return handlers;
        }

        public bool MatchHandlerExist()
        {
            return GetHandlers().Where(x => x.Match()).Any();
        }

        public IMessageHandler GetMatchHandler()
        {
            return GetHandlers().Where(x => x.Match()).FirstOrDefault();
        }
    }
}

using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Bot.MessageHandlers.InlineQuery;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public class BuilderForInlineQuery : BuilderHandlerBase
    {
        public BuilderForInlineQuery(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
            AddAll();
        }

        private void AddAll()
        {
            handlers.Add(AddInlineQueryHandler());
            handlers.Add(AddInlineResultHandler());
        }
        private IMessageHandler AddInlineQueryHandler()
        {
            return new InlineQueryHandler(botSubSystems);
        }

        private IMessageHandler AddInlineResultHandler()
        {
            return new InlineResultHandler(botSubSystems);
        }
    }
}

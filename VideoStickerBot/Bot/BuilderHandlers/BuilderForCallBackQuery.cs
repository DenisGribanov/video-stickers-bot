using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Bot.MessageHandlers.CallBackQuery;
using VideoStickerBot.Bot.MessageHandlers.CallBackQuery.ReviewVideoSticker;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerPublishing;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public class BuilderForCallBackQuery : BuilderHandlerBase
    {
        public BuilderForCallBackQuery(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
            AddAll();
        }

        private void AddAll()
        {
            handlers.Add(GetSortTypeEnterHandler());
            handlers.Add(GetReviewerRejectedHandler());
            handlers.Add(GetReviewerAprovedHandler());
        }


        private IMessageHandler GetSortTypeEnterHandler()
        {
            return new SortTypeEnterHandler(botSubSystems);
        }

        private IMessageHandler GetReviewerRejectedHandler()
        {
            return new ReviewerRejectedHandler(botSubSystems);
        }

        private IMessageHandler GetReviewerAprovedHandler()
        {
            return new ReviewerAprovedHandler(botSubSystems);
        }
    }
}

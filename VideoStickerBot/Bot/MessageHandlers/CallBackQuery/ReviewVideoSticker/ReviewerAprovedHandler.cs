using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.StickerPublishing;

namespace VideoStickerBot.Bot.MessageHandlers.CallBackQuery.ReviewVideoSticker
{
    public class ReviewerAprovedHandler : ReviewBaseHandler
    {
        private readonly IStickerPublishing StickerPublishing;

        protected override VideoReviewEnum VideoReviewAction => VideoReviewEnum.APPROVAL;

        public ReviewerAprovedHandler(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
            StickerPublishing = new StickerPublishingForReview(botSubSystems.Telegram, botSubSystems.TelegramUpdateMessage, botSubSystems.DataStore);
        }

        public override bool Match()
        {
            isMatchForTelegramUpdate = base.Match();

            if (!isMatchForTelegramUpdate.Value)
            {
                return false;
            }

            isMatchForTelegramUpdate = KeyboadCallBackData.Data.ReviewResult == VideoReviewEnum.APPROVAL;

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            if (GetVideoStickers().IsPublished())
            {
                await IsPublishing();

                return;
            }

            await StickerPublishing.Publish(GetVideoStickers());

            SaveReviewResult();
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.REVIEWER_APROVED;
        }
    }
}
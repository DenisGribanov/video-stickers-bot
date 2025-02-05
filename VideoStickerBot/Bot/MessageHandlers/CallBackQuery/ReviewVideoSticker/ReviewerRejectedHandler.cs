using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.CallBackQuery.ReviewVideoSticker
{
    public class ReviewerRejectedHandler : ReviewBaseHandler
    {
        protected override VideoReviewEnum VideoReviewAction => VideoReviewEnum.REJECTION;

        public ReviewerRejectedHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            isMatchForTelegramUpdate = base.Match();

            if (!isMatchForTelegramUpdate.Value)
            {
                return false;
            }

            isMatchForTelegramUpdate = KeyboadCallBackData.Data.ReviewResult == VideoReviewEnum.REJECTION;

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

            await Reject();

            SaveReviewResult();
        }

        private async Task Reject()
        {
            var post = GetVideoStickers().GetPrivateChannelPost();

            await Telegram.SendTextMessage($"\n\n ❌ - пользователь @{CurrentUser.UserName}", post.ChannelId, post.ReplyMessageId);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.REVIEWER_REJECTED;
        }
    }
}
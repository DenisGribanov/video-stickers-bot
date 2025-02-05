using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.CallBackQuery.ReviewVideoSticker
{
    public abstract class ReviewBaseHandler : BaseMessageHandler
    {
        protected KeyboadBaseDto<ReviewResultDto> KeyboadCallBackData { get; private set; }
        private VideoSticker sticker;
        protected abstract VideoReviewEnum VideoReviewAction { get; }

        protected ReviewBaseHandler(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (!TelegramUpdate.IsCallBackQuery || string.IsNullOrEmpty(TelegramUpdate.CallBackData))
            {
                isMatchForTelegramUpdate = false;
                return isMatchForTelegramUpdate.Value;
            }

            if (GetKeyboardType(TelegramUpdate.CallBackData) == KeyboardTypeEnum.VIDEO_REVIEW_RESULT
                && (CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer()))
            {
                isMatchForTelegramUpdate = true;
                KeyboadCallBackData = KeyboadBaseDto<ReviewResultDto>.FromJson(TelegramUpdate.CallBackData);
                return isMatchForTelegramUpdate.Value;
            }
            else
            {
                isMatchForTelegramUpdate = false;
                return isMatchForTelegramUpdate.Value;
            }
        }

        protected VideoSticker GetVideoStickers()
        {
            if (sticker != null) return sticker;

            sticker = DataStore.GetVideoStickers().First(x => x.Id == KeyboadCallBackData.Data.VideoStickerId);
            return sticker;
        }

        protected async Task IsPublishing()
        {
            await Telegram.AnswerCallbackQuery(TelegramUpdate.CallBackQueryId, $"Уже опубликовано");

            if (sticker.ChannelPosts == null) return;

            var post = sticker.ChannelPosts.Where(x => x.Channel.ChannelType == (int)ChannelType.PUBLIC).First();

            await Telegram.SendTextMessage(post.VideoUrl, CurrentUser.ChatId);
        }

        protected void SaveReviewResult()
        {
            DataStore.AddCheckingVideo(new CheckingVideoSticker
            {
                DateAdd = DateTime.Now,
                ModeratorChatId = CurrentUser.ChatId,
                Status = (int)VideoReviewAction,
                StatusUpdateTime = DateTime.Now,
                VideoStickerId = sticker.Id,
                ModeratorChat = CurrentUser
            });
        }
    }
}
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public class StickerHashTagInputHandler : CmdStickerManagmentBaseHandler
    {
        public StickerHashTagInputHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (CurrentUser != null && !TelegramUpdate.IsBotCommand && !string.IsNullOrEmpty(TelegramUpdate.MessageText)
                && (CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer()))
            {
                stickerId = GetVideoStickerIdFromState(BotState.EDIT_HASHTAG);

                isMatchForTelegramUpdate = stickerId.HasValue;
            }
            else
            {
                isMatchForTelegramUpdate = false;
            }

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            sticker = GetSticker(stickerId.Value);

            if (sticker == null)
            {
                await Telegram.SendTextMessage("Стикер не найден 🤷🏻‍♂️", CurrentUser.ChatId);
                return;
            }

            sticker.Hashtags = TelegramUpdate.MessageText.Replace("#", "").Trim();

            foreach (var chPost in sticker.ChannelPosts)
            {
                if (!chPost.ReplyMessageId.HasValue) continue;

                if (chPost.Channel.ChannelType == (int)ChannelType.PUBLIC)
                    await Telegram.EditMessageReplyMarkup(chPost.ReplyMessageId.Value,
                        chPost.Channel.Id,
                        sticker.BuildPostDescriptionText());

                if (chPost.Channel.ChannelType == (int)ChannelType.PRIVATE_REVIEW)
                    await Telegram.EditMessageReplyMarkup(chPost.ReplyMessageId.Value,
                        chPost.Channel.Id,
                        sticker.BuildPostDescriptionText(),
                        sticker.GetReviewKeybooardData());
            }

            DataStore.UpdateVideoSticker(sticker);

            await Telegram.SendTextMessage("Успешно сохранено ✅\n\n" +
                $"Посмотреть {BotCommands.ID}{stickerId.Value}", CurrentUser.ChatId);

            ResetFromState(BotState.EDIT_HASHTAG);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.INPUT_NEW_HASHTAG;
        }
    }
}
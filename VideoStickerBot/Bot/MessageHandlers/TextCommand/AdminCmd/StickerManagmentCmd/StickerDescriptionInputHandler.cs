using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public class StickerDescriptionInputHandler : CmdStickerManagmentBaseHandler
    {
        public StickerDescriptionInputHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (CurrentUser != null && !TelegramUpdate.IsBotCommand && !string.IsNullOrEmpty(TelegramUpdate.MessageText)
                && (CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer()))
            {
                stickerId = GetVideoStickerIdFromState(BotState.EDIT_DESCRIPTION);

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

            sticker.Description = TelegramUpdate.MessageText.Trim();

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

                logger.Info("update text from channel post success");
            }

            DataStore.UpdateVideoSticker(sticker);

            await Telegram.SendTextMessage("Успешно сохранено ✅\n\n" +
                $"Посмотреть {BotCommands.ID}{stickerId.Value}", CurrentUser.ChatId);

            ResetFromState(BotState.EDIT_DESCRIPTION);
        }


        protected override BotState GetHandlerStateName()
        {
            return BotState.INPUT_NEW_DESCRIPTION;
        }
    }
}

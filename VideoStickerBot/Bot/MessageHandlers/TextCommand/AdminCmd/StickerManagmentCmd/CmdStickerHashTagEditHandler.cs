using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public class CmdStickerHashTagEditHandler : CmdStickerManagmentBaseHandler
    {
        public CmdStickerHashTagEditHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }


        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Contains(BotCommands.EDIT_HASHTAG))
            {
                stickerId = DigitParse(TelegramUpdate.MessageText);

                isMatchForTelegramUpdate = stickerId.HasValue
                                            && CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer();
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

            SaveVideoStickerIdFromState(GetHandlerStateName(), stickerId.Value);

            await Telegram.SendTextMessage($"Текущие хэштеги: _{sticker.Hashtags}_\n\n" +
                $"🆕 Пришлите новые хэштеги ", CurrentUser.ChatId);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.EDIT_HASHTAG;
        }
    }
}

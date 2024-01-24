using Telegram.Bot.Types;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public class CmdStickerDescriptionEditHandler : CmdStickerManagmentBaseHandler
    {
        public CmdStickerDescriptionEditHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Contains(BotCommands.EDIT_DESCRIPTION))
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

            await Telegram.SendTextMessage($"Текущее описание: _{sticker.Description}_\n\n" +
                $"🆕 Пришлите новое описание ", CurrentUser.ChatId);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.EDIT_DESCRIPTION;
        }
    }
}

using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdAddHandler : BaseMessageHandler
    {
        public CmdAddHandler(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand
                                        && TelegramUpdate.MessageText.Contains(BotCommands.ADD_VIDEO);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            await Telegram.SendTextMessage("Пришлите кружочек 🔵 или обычное видео 🎞 (я сам преобразую 🔄 его в кружок)", TelegramUpdate.ChatId.Value);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.ADD_VIDEO;
        }
    }
}
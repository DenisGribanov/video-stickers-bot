using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdBestHandler : BaseMessageHandler
    {
        public CmdBestHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            List<List<KeyValuePair<string, string>>> keyboard = new List<List<KeyValuePair<string, string>>>();
            keyboard.Add(new List<KeyValuePair<string, string>>());
            keyboard.LastOrDefault().Add(new KeyValuePair<string, string>("Жми сюда 🙃", $"{CashTagValues.BEST}"));

            if (TelegramUpdate.ChatId.HasValue)
            {
                await Telegram.SendTextMessage("Самые популярные кружочки 🔥", TelegramUpdate.ChatId.Value, keyboard);
            }
            else
            {
                await Telegram.SendTextMessage("Самые популярные кружочки 🔥", CurrentUser.ChatId, keyboard);
            }
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            // /best@VideoStickersBot
            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand &&
                                       TelegramUpdate.MessageText.Contains(BotCommands.BEST.Replace("/", ""));

            return isMatchForTelegramUpdate.Value;
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.FRESH;
        }
    }
}
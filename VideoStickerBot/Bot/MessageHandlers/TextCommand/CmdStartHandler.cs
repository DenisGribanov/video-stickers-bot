using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdStartHandler : BaseMessageHandler
    {
        public CmdStartHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {

        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Equals(BotCommands.START_BOT);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;


            await Telegram.SendTextMessage("Команды 👇" +
                $"\n\n❓ Как пользоваться ? {BotCommands.HELP}" +
                $"\n\n📋 Подборки кружочков {BotCommands.COMPILATION}" +
                $"\n\n⚙️ Настройки {BotCommands.SETTINGS}", TelegramUpdate.ChatId.Value);


        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.START;
        }
    }
}

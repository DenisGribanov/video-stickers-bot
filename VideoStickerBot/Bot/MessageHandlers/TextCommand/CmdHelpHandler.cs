using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdHelpHandler : BaseMessageHandler
    {
        public CmdHelpHandler(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {

        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand
                                        && TelegramUpdate.MessageText.Contains(BotCommands.HELP);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;


            List<List<KeyValuePair<string, string>>> keyboard = new List<List<KeyValuePair<string, string>>>();
            keyboard.Add(new List<KeyValuePair<string, string>>());
            keyboard.LastOrDefault().Add(new KeyValuePair<string, string>("Жми сюда 🙃", $""));

            await Telegram.SendVideo(Variables.GetInstance().VIDEO_HELP_FILE_ID, TelegramUpdate.ChatId.Value, keyboard);
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.HELP;
        }
    }
}

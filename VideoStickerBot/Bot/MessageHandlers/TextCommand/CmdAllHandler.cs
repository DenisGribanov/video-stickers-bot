using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdAllHandler : BaseMessageHandler
    {
        public CmdAllHandler(IBotSubSystems botSubSystems) 
            : base(botSubSystems)
        {

        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Contains(BotCommands.ALL_VIDEO);

            return isMatchForTelegramUpdate.Value;
        }

        public async override Task Handle()
        {
            if (!Match()) return;

            await Telegram.SendTextMessage(TextWithUrl("Кружочки для переговоров",Variables.GetInstance().PUBLIC_CHANNEL_URL),
                                        TelegramUpdate.ChatId.Value);

        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.VIEW_ALL_VIDEO;
        }
    }
}

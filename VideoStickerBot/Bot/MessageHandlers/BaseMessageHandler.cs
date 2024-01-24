using NLog;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.Handlers
{
    public abstract class BaseMessageHandler : IMessageHandler
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly ITelegram Telegram;

        protected readonly IDataStore DataStore;

        protected readonly ITelegramUpdateMessage TelegramUpdate;

        private readonly IStateData StateData;

        protected bool? isMatchForTelegramUpdate;

        protected TgUser? CurrentUser => StateData.CurrentUser;

        protected BotState? StateCurrentUser => StateData.StateCurrentUser;

        protected BaseMessageHandler(IBotSubSystems botSubSystems)
        {
            Telegram = botSubSystems.Telegram;
            TelegramUpdate = botSubSystems.TelegramUpdateMessage;
            DataStore = botSubSystems.DataStore;
            StateData = botSubSystems.StateData;
        }

        public abstract bool Match();
        public abstract Task Handle();
        protected abstract BotState GetHandlerStateName();

        public BotState? UpdateStateForCurrentUser()
        {
            return StateData.UpdateState(GetHandlerStateName());
        }

        protected static KeyboardTypeEnum? GetKeyboardType(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var dto = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyboadBaseDto<object>>(json);

            return dto.Type;
        }

        protected int? DigitParse(string text)
        {
            Regex regex = new Regex("(\\d+)");

            var regexMatch = regex.Match(text);

            return regexMatch.Success ? Convert.ToInt32(regexMatch.Value) : null;
        }

        protected string TextWithUrl(string text, string url)
        {
            return $"[{text}]({url})";
        }
    }
}

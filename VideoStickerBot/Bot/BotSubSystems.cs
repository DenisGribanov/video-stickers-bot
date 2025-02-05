using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.VideoResize;

namespace VideoStickerBot.Bot
{
    public class BotSubSystems : IBotSubSystems
    {
        public ITelegram Telegram { get; private set; }

        public IDataStore DataStore { get; private set; }

        public ITelegramUpdateMessage TelegramUpdateMessage { get; private set; }

        public IStateData StateData { get; private set; }

        public IVideoResize VideoResize { get; private set; }

        public BotSubSystems(ITelegram telegram,
            IDataStore dataStore,
            ITelegramUpdateMessage telegramUpdateMessage,
            IVideoResize videoResize,
            IStateData stateData)
        {
            Telegram = telegram;
            DataStore = dataStore;
            TelegramUpdateMessage = telegramUpdateMessage;
            StateData = stateData;
            VideoResize = videoResize;
        }
    }
}
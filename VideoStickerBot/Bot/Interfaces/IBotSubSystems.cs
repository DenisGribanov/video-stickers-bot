using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.VideoResize;

namespace VideoStickerBot.Bot.Interfaces
{
    public interface IBotSubSystems
    {
        ITelegram Telegram { get; }

        IDataStore DataStore { get; }

        ITelegramUpdateMessage TelegramUpdateMessage { get; }

        IStateData StateData { get; }

        IVideoResize VideoResize { get; }
    }
}
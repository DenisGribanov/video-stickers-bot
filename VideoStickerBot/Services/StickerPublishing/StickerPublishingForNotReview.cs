using Telegram.Bot.Types;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Services.StickerPublishing
{
    public class StickerPublishingForNotReview : StickerPublishingBase
    {
        public StickerPublishingForNotReview(ITelegram telegram,
            ITelegramUpdateMessage telegramUpdate,
            IDataStore dataStore) : base(telegram, telegramUpdate, dataStore)
        {

        }

        public override async Task Publish(VideoSticker _sticker)
        {
            if (_sticker == null) throw new ArgumentNullException(nameof(sticker));

            sticker = _sticker;

            publicChannel = GetPublicChannel();

            privateChannel = GetPrivateChannel();

            await SendToPublicChannel();

            await SendNotifyToPrivateChannel();

            await SendNotifyToAuthor();

        }


        private async Task SendNotifyToPrivateChannel()
        {
            await Telegram.SendTextMessage($"Пользователь @{TelegramUpdate.Username} добавил кружочек:" +
                                            $"\n\n[Посмотреть в канале]({UrlPublicChannelPost()})", privateChannel.Id);
        }

    }
}

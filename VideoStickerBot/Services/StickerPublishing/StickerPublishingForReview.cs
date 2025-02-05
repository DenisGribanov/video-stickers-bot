using VideoStickerBot.Database;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Services.StickerPublishing
{
    public class StickerPublishingForReview : StickerPublishingBase
    {
        public StickerPublishingForReview(ITelegram telegram, ITelegramUpdateMessage telegramUpdate, IDataStore dataStore)
            : base(telegram, telegramUpdate, dataStore)
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
            var postPrivateChannel = sticker.GetPrivateChannelPost();

            await Telegram.SendTextMessage($"Пользователь @{TelegramUpdate.Username} одобрил кружочек:\n\n" +
                                        $"[Посмотреть]({UrlPublicChannelPost()})",
                                         postPrivateChannel.ChannelId,
                                         postPrivateChannel.ReplyMessageId);
        }
    }
}
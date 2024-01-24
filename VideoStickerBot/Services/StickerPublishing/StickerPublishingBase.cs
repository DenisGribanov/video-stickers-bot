using Microsoft.VisualBasic;
using NLog;
using Telegram.Bot.Types;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Services.StickerPublishing
{
    public abstract class StickerPublishingBase : IStickerPublishing
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly ITelegram Telegram;
        protected readonly IDataStore DataStore;
        protected readonly ITelegramUpdateMessage TelegramUpdate;

        protected VideoSticker sticker;
        protected Channel privateChannel;
        protected Channel publicChannel;

        private int videoNotePublicMessageId;

        protected StickerPublishingBase(ITelegram telegram,
            ITelegramUpdateMessage telegramUpdate,
            IDataStore dataStore)
        {
            Telegram = telegram;
            DataStore = dataStore;
            TelegramUpdate = telegramUpdate;
        }

        public abstract Task Publish(VideoSticker _sticker);

        protected async Task SendToPublicChannel()
        {
            videoNotePublicMessageId = await SendVideoNoteToPublicChannel();

            var descriptionMessageId = await SendReplyTextMessage(videoNotePublicMessageId);

            SavePublicChannelPost(descriptionMessageId);

        }

        private void SavePublicChannelPost(int ReplyMessageId)
        {
            DataStore.AddChannelPost(new ChannelPost
            {
                Channel = publicChannel,
                ChannelId = publicChannel.Id,
                MessageId = videoNotePublicMessageId,
                ReplyMessageId = ReplyMessageId,
                VideoStickerId = sticker.Id,
                VideoUrl = VideoTelescopeUrl(),
                Deleted = false
            });
        }

        private async Task<int> SendVideoNoteToPublicChannel()
        {
            int msgId = 0;
            try
            {
                var message = await Telegram.ForwardMessage(publicChannel.Id, sticker.AuthorChatId, sticker.MessageId);
                logger.Info("SendVideoNoteToPublicChannel Success (ForwardMessage)");
                msgId = message.MessageId.Value;
            } catch (Exception ex)
            {
                logger.Error(ex);
                var message = await Telegram.SendVideoNote(sticker.FileId, publicChannel.Id);
                msgId = message.MessageId.Value;
            }

            return msgId;
        }

        private async Task<int> SendReplyTextMessage(int replyMessageId)
        {
            var descriptionMessage = await Telegram.SendTextMessage(sticker.BuildPostDescriptionText(), publicChannel.Id, replyMessageId);

            return descriptionMessage.MessageId.Value;
        }

        protected async Task SendNotifyToAuthor()
        {
            List<List<KeyValuePair<string, string>>> buttonData = new()
            {
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Посмотреть", sticker.Id.ToString())
                }
            };

            await Telegram.SendTextMessage($"Кружочек опубликован", sticker.AuthorChatId, buttonData);
        }

        protected Channel GetPrivateChannel()
        {
            return DataStore.GetChannels()
                    .Where(x => x.ChannelType == (int)ChannelType.PRIVATE_REVIEW)
                    .First();
        }

        protected Channel GetPublicChannel()
        {
            return DataStore.GetChannels()
                    .Where(x => x.ChannelType == (int)ChannelType.PUBLIC)
                    .First();
        }

        protected string UrlPublicChannelPost()
        {
            //https://t.me/best_circle/20
            return $"{Variables.T_ME_URL}/{publicChannel.Name}/{videoNotePublicMessageId}";
        }

        protected string VideoTelescopeUrl()
        {
            return $"{Variables.TELESCOPE_URL}/{publicChannel.Name}/{videoNotePublicMessageId}";
        }
    }
}

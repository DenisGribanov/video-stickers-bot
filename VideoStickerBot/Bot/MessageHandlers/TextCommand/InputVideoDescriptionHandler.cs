using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerPublishing;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class InputVideoDescriptionHandler : BaseMessageHandler
    {
        const int DESC_MAX_LEN = 100;
        VideoSticker sticker;
        IStickerPublishing StickerPublishing;

        public InputVideoDescriptionHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
            StickerPublishing = new StickerPublishingForNotReview(botSubSystems.Telegram, botSubSystems.TelegramUpdateMessage, botSubSystems.DataStore);
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = BotState.VIDEO_UPLOADED.Equals(StateCurrentUser)
                                        && !string.IsNullOrEmpty(TelegramUpdate.MessageText);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            if (!await Validate()) return;

            sticker = GetVideoSticker();

            SaveVideoNoteDescription();

            //без проверки. сразу публикуем
            if (CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer())
                await StickerPublishing.Publish(sticker);
            else
                await SendToReview();

        }

        private VideoSticker GetVideoSticker()
        {
            //найти последний добавленный стикер от тек. пользователя
            return DataStore.GetVideoStickers().Where(x => x.AuthorChatId == CurrentUser.ChatId
                                                        && !x.IsPublished() && string.IsNullOrEmpty(x.Description))
                                                            .Last();
        }

        private async Task<bool> Validate()
        {
            if (TelegramUpdate.MessageText.Length > DESC_MAX_LEN)
            {
                await Telegram.SendTextMessage($"Ошибка ⚠️ Максимальная длина описания {DESC_MAX_LEN} символов", CurrentUser.ChatId);
                return false;
            }

            return true;
        }

        private void SaveVideoNoteDescription()
        {
            const string pattern = "(#(?:[^\\x00-\\x7F]|\\w)+)";

            string MessageText = TelegramUpdate.MessageText;

            string Description = MessageText;
            List<string> hashTags = new List<string>();

            foreach (Match match in Regex.Matches(MessageText, pattern, RegexOptions.None))
            {
                Console.WriteLine(match.Value.Replace("#", ""), match.Index);
                hashTags.Add(match.Value.Replace("#", "").Replace("\n", "").Trim());

                Description = Description.Replace(match.Value, "");
            }

            sticker.Description = Description.Replace("\n", "").Trim();
            sticker.Hashtags = string.Join(" ", hashTags.ToArray());

            DataStore.UpdateVideoSticker(sticker);

        }

        private async Task SendToReview()
        {
            var privateChannel = GetPrivateChannel();

            var message = await Telegram.SendVideoNote(sticker.FileId, privateChannel.Id);
            var msgVideoNoteId = message.MessageId.Value;

            string replyText = $"Пользователь @{TelegramUpdate.Username} предложил новое видео id:{sticker.Id}" +
                $"\n\nОписание 👇👇👇\n\n" +
                $"{sticker.Description}\n\n" +
                $"хэштеги : {sticker.Hashtags}";

            var replyMessage = await Telegram.SendTextMessage(replyText, privateChannel.Id, msgVideoNoteId,
                                                                sticker.GetReviewKeybooardData());


            DataStore.AddChannelPost(new ChannelPost
            {
                Channel = privateChannel,
                ChannelId = privateChannel.Id,
                MessageId = msgVideoNoteId,
                VideoStickerId = sticker.Id,
                ReplyMessageId = replyMessage.MessageId,
                DateAdd = DateTime.Now,
                PostText = replyText,
                VideoUrl = string.Empty,
                Deleted = false,
            });

            await Telegram.SendTextMessage("Отправлено на модерацию ✅", CurrentUser.ChatId);
        }


        private Channel GetPrivateChannel()
        {
            return DataStore.GetChannels()
                    .Where(x => x.ChannelType == (int)ChannelType.PRIVATE_REVIEW)
                    .First();
        }


        protected override BotState GetHandlerStateName()
        {
            return BotState.ADD_VIDEO_DESCRIPTION;
        }
    }
}

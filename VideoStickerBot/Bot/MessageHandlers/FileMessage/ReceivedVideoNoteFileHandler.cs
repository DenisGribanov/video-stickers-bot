using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.FileMessage
{
    public class ReceivedVideoNoteFileHandler : ReceivedFileHandlerBase
    {
        public ReceivedVideoNoteFileHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = BotState.ADD_VIDEO == StateCurrentUser
                                       && TelegramFileType.VIDEO_NOTE.Equals(TelegramUpdate.FileType)
                                       && !CurrentUser.UploadDisabled;

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            if (!await Validate()) return;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await Telegram.SendTextMessage("Работаю 😊😊😊", CurrentUser.ChatId);

                await DownloadVideoFile(TelegramUpdate.FileId, memoryStream);

                await SaveFile(memoryStream.ToArray(), TelegramUpdate.FileId + ".mp4", Variables.GetInstance().CACHE_FOLDER);

                memoryStream.Position = 0;
                var message = await Telegram.SendVideoNote(memoryStream, CurrentUser.ChatId);

                SaveVideoInfo(message.FileId, message.FileUniqueId, message.MessageId.Value);
            }

            await SendMessageRequestingDescription();
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.VIDEO_UPLOADED;
        }
    }
}
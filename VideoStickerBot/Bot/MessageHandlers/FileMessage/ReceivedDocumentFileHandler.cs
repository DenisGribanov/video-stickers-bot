using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.FileMessage
{
    public class ReceivedDocumentFileHandler : ReceivedFileHandlerBase
    {
        public ReceivedDocumentFileHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = BotState.ADD_VIDEO == StateCurrentUser &&
                                       TelegramFileType.FILE_DOCUMENT.Equals(TelegramUpdate.FileType)
                                        && !CurrentUser.UploadDisabled;

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            if (!await Validate()) return;

            await ConvertToVideoNote();
        }

        protected override async Task<bool> Validate()
        {
            var res = await base.Validate();

            if (!res) return false;

            if (TelegramUpdate.FileSize.HasValue && TelegramUpdate.FileSize.Value > MAX_FILE_SIZE)
            {
                await Telegram.SendTextMessage("Ошибка ⚠️ Файл должен быть не более 15 мб", CurrentUser.ChatId);
                return false;
            }

            if (!"video/mp4".Equals(TelegramUpdate.FileMimeType))
            {
                await Telegram.SendTextMessage("Ошибка ⚠️ Файл должен быть в формате mp4", CurrentUser.ChatId);
                return false;
            }

            return true;
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.VIDEO_UPLOADED;
        }
    }
}
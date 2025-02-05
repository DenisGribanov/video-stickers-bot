using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.FileMessage
{
    public class ReceivedVideoFileHandler : ReceivedFileHandlerBase
    {
        public ReceivedVideoFileHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramFileType.VIDEO.Equals(TelegramUpdate.FileType)
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

            return true;
        }
    }
}
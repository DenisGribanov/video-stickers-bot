using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.VideoResize;

namespace VideoStickerBot.Bot.MessageHandlers.FileMessage
{
    public abstract class ReceivedFileHandlerBase : BaseMessageHandler
    {
        protected const int MAX_VIDEO_DURATION = 20;
        protected const int MAX_REVIEWS = 5;
        protected const int MAX_FILE_SIZE = 10000000;

        protected readonly IVideoResize videoResize;

        public ReceivedFileHandlerBase(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
            videoResize = botSubSystems.VideoResize;
        }

        protected void SaveVideoInfo(string fileId, string fileUniqueId, int messageId)
        {
            DataStore.AddVideoSticker(
            new Database.VideoSticker
            {
                AuthorChatId = CurrentUser.ChatId,
                FileId = fileId,
                DateAdd = DateTime.Now,
                VideoDuration = TelegramUpdate.VideoDuration.HasValue ? TelegramUpdate.VideoDuration.Value : 0,
                MessageId = messageId,
                FileUniqueId = fileUniqueId,
                AuthorChat = CurrentUser
            });
        }

        protected async Task SendMessageRequestingDescription()
        {
            await Telegram.SendTextMessage(
                    "Теперь пришлите описание (потребуется для быстрого поиска 🔎)" +
                    "\n\nНапример:" +
                    "\n\n_Государство это мы_" +
                    "\n#бригада", CurrentUser.ChatId);
        }

        protected virtual async Task<bool> Validate()
        {
            if (DataStore.GetVideoStickers().Any(x => x.IsPublished() && TelegramUpdate.FileUniqueId.Equals(x.FileUniqueId)))
            {
                await Telegram.SendTextMessage($"Этот кружочек уже добавлен ⚠️", CurrentUser.ChatId);
                return false;
            }

            if (TelegramUpdate.VideoDuration.HasValue && TelegramUpdate.VideoDuration.Value > MAX_VIDEO_DURATION)
            {
                await Telegram.SendTextMessage($"Ошибка ⚠️ Максимальная продолжительность видео {MAX_VIDEO_DURATION} сек.", CurrentUser.ChatId);
                return false;
            }

            if (TelegramUpdate.FileSize.HasValue && TelegramUpdate.FileSize.Value > MAX_FILE_SIZE)
            {
                await Telegram.SendTextMessage($"Ошибка ⚠️ Максимальный размер видео 10 мб.", CurrentUser.ChatId);
                return false;
            }

            var notPublishedVideos = DataStore.GetVideoStickers()
                .Where(x => !x.Deleted && !string.IsNullOrEmpty(x.Description)
                && x.AuthorChatId == CurrentUser.ChatId
                && !x.IsPublished() && (x.CheckingVideoStickers == null || x.CheckingVideoStickers.Count == 0))
                .Count();

            if (notPublishedVideos >= MAX_REVIEWS)
            {
                await Telegram.SendTextMessage($"Ошибка ⚠️. У вас уже {notPublishedVideos} стикеров на проверке.", CurrentUser.ChatId);
                return false;
            }

            return true;
        }

        protected async Task ConvertToVideoNote()
        {
            using MemoryStream sourceVideo = new();

            await DownloadVideoFile(TelegramUpdate.FileId, sourceVideo);

            await Telegram.SendTextMessage("Идет обработка 🔄. Пожалуйста подождите 🙏", CurrentUser.ChatId);

            var squareVideo = await videoResize.ConvertToSquareAsync(sourceVideo);

            await SaveFile(squareVideo, TelegramUpdate.FileId + ".mp4", Variables.GetInstance().CACHE_FOLDER);

            var message = await Telegram.SendVideoNote(new MemoryStream(squareVideo), CurrentUser.ChatId);

            SaveVideoInfo(message.FileId, message.FileUniqueId, message.MessageId.Value);

            await SendMessageRequestingDescription();
        }

        protected async Task DownloadVideoFile(string fileId, MemoryStream memoryStream)
        {
            await Telegram.GetFile(fileId, memoryStream);
        }

        protected async Task SaveFile(byte[] bytes, string fileName, string path = null)
        {
            try
            {
                if (bytes == null) throw new ArgumentNullException("bytes");

                string pathSave = path == null ? fileName : Path.Combine(path, fileName);

                await System.IO.File.WriteAllBytesAsync(pathSave, bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при сохранении файла");
            }
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.VIDEO_UPLOADED;
        }
    }
}
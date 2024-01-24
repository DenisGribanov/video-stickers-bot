using Telegram.Bot.Types;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Services.TelegramIntegration
{
    public class TelegramUpdateMessageAdapter : ITelegramUpdateMessage
    {
        private readonly Update? update;

        public TelegramUpdateMessageAdapter(Update update)
        {
            this.update = update;
        }

        public TelegramUpdateMessageAdapter(Message message)
        {
            this._msg = message;
        }

        public long UserFromId
        {
            get
            {
                if (update.Message != null && update.Message.From != null)
                {
                    return update.Message.From.Id;
                }
                else if (update.ChosenInlineResult != null && update.ChosenInlineResult.From != null)
                {
                    return update.ChosenInlineResult.From.Id;
                }
                else if (update.InlineQuery != null && update.InlineQuery.From != null)
                {
                    return update.InlineQuery.From.Id;
                }
                else if (update.CallbackQuery != null)
                {
                    return update.CallbackQuery.From.Id;
                }
                else if (update.MyChatMember != null)
                {
                    return update.MyChatMember.From.Id;
                }
                else
                {
                    return 0;
                }

            }
        }

        public string Username
        {
            get
            {
                var name = message != null ? message.Chat?.Username : update.InlineQuery?.From.Username;

                if (update.CallbackQuery != null)
                {
                    name = update.CallbackQuery.From.Username;
                }
                else if (update.ChosenInlineResult != null)
                {
                    name = update.ChosenInlineResult.From?.Username;
                }

                return string.IsNullOrEmpty(name) ? "UNKNOWN" : name;
            }
        }


        public string InlineQueryText => update?.InlineQuery?.Query;

        public string InlineQueryOffset => update?.InlineQuery?.Offset;

        public bool IsInlineQuery => update.Type.Equals(Telegram.Bot.Types.Enums.UpdateType.InlineQuery);

        public bool IsChannelPost => update.Type.Equals(Telegram.Bot.Types.Enums.UpdateType.ChannelPost);


        public int? ReplyMessageId => message?.ReplyToMessage?.MessageId;

        public string? MessageText => message?.Text;

        public int? MessageId
        {
            get
            {
                return message != null ? message?.MessageId : update.CallbackQuery.Message.MessageId;
            }
        }


        public bool IsChosenInlineResult => update.Type == Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult;

        public string? ChosenInlineResultId => update.ChosenInlineResult?.ResultId;

        private Message? message
        {
            get
            {
                if (_msg != null)
                    return _msg;
                else if (update.Message != null)
                    return update.Message;
                else if (update.ChannelPost != null && update.Message != null)
                    return update.ChannelPost;
                else if (update.EditedMessage != null)
                    return update.EditedMessage;
                else if (update.EditedChannelPost != null)
                    return update.EditedChannelPost;
                else
                    return null;

            }
        }

        private Message? _msg;

        public TelegramFileType? FileType
        {
            get
            {
                if (message == null)
                    return null;

                if (message.Video != null)
                    return TelegramFileType.VIDEO;
                else if (message.VideoNote != null)
                    return TelegramFileType.VIDEO_NOTE;
                else if (message.Document != null)
                    return TelegramFileType.FILE_DOCUMENT;
                else
                    return null;
            }
        }

        public string? FileMimeType
        {
           get
            {
                return message?.Document?.MimeType;
            }
        }

        public string? FileId
        {
            get
            {
                if (FileType == null)
                    return null;

                if (FileType == TelegramFileType.VIDEO_NOTE)
                    return message.VideoNote.FileId;
                else if (FileType == TelegramFileType.VIDEO)
                    return message.Video.FileId;
                else if (FileType == TelegramFileType.FILE_DOCUMENT)
                    return message.Document.FileId;
                else
                    return null;
            }
        }

        public string FileUniqueId
        {
            get
            {
                if (FileType == null)
                    return null;

                if (FileType == TelegramFileType.VIDEO_NOTE)
                    return message.VideoNote.FileUniqueId;
                else if (FileType == TelegramFileType.VIDEO)
                    return message.Video.FileUniqueId;
                else if (FileType == TelegramFileType.FILE_DOCUMENT)
                    return message.Document.FileUniqueId;
                else
                    return null;
            }
        }

        public int? VideoDuration
        {
            get
            {
                if (FileType == null)
                    return null;

                if (FileType == TelegramFileType.VIDEO_NOTE)
                    return message.VideoNote.Duration;
                else if (FileType == TelegramFileType.VIDEO)
                    return message.Video.Duration;
                else
                    return null;
            }
        }


        public long? FileSize
        {
            get
            {
                if (FileType == null)
                    return null;

                if (FileType == TelegramFileType.VIDEO_NOTE)
                    return message.VideoNote.FileSize;
                else if (FileType == TelegramFileType.VIDEO)
                    return message.Video.FileSize;
                else if (FileType == TelegramFileType.FILE_DOCUMENT)
                    return message.Document.FileSize;
                else
                    return null;
            }
        }

        public bool IsBotCommand
        {
            get
            {
                if (message == null || message.Entities == null) return false;

                return message.Entities.Where(x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.BotCommand).Any();
            }
        }

        public bool IsCallBackQuery => update.CallbackQuery != null;

        public string? CallBackData => update.CallbackQuery?.Data;

        public string? InlineQueryId => update?.InlineQuery?.Id;

        public string? CallBackQueryId => update?.CallbackQuery.Id;

        public long? ChatId => message.Chat?.Id;

        public override string? ToString()
        {
            if(update != null)
            {
               return  Newtonsoft.Json.JsonConvert.SerializeObject(update);
            }

            return null;
        }
    }
}
